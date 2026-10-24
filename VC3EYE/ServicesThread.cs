using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE
{
    public class ServicesThread : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServicesThread> _logger;

        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);
        private const int MaxConcurrentChecks = 4;

        public ServicesThread(IServiceScopeFactory scopeFactory, ILogger<ServicesThread> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckDueServicesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // Keep the hosted worker alive if one polling pass fails.
                    _logger.LogError(ex, "Service monitoring pass failed");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private async Task CheckDueServicesAsync(CancellationToken stoppingToken)
        {
            List<int> dueServiceIds;
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Vc3eyeContext>();
                var now = DateTime.Now;
                var services = await db.Services
                    .Where(s => s.IsActive && !s.IsDeleted)
                    .Select(s => new { s.ServiceId, s.LastTimeChecked, s.CheckInterval })
                    .ToListAsync(stoppingToken);

                dueServiceIds = services
                    .Where(s => s.LastTimeChecked + s.CheckInterval <= now)
                    .Select(s => s.ServiceId)
                    .ToList();
            }

            if (dueServiceIds.Count == 0)
                return;

            await Parallel.ForEachAsync(
                dueServiceIds,
                new ParallelOptions
                {
                    CancellationToken = stoppingToken,
                    MaxDegreeOfParallelism = MaxConcurrentChecks
                },
                async (serviceId, cancellationToken) =>
                {
                    // Each check gets its own scope and DbContext. DbContext is
                    // not thread-safe, so no context or tracked entity is shared.
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Vc3eyeContext>();
                    var manager = new ServiceManager(db);
                    try
                    {
                        var service = await manager.GetServiceByIDAsync(serviceId, cancellationToken);
                        if (service == null || !service.IsActive || service.IsDeleted)
                            return;

                        await CheckServiceAsync(manager, service, cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Health check failed for service {ServiceId}", serviceId);
                    }
                });
        }

        private async Task CheckServiceAsync(ServiceManager manager, Service service, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var http = string.IsNullOrWhiteSpace(service.Url)
                ? new HttpCheckResult(false, null, "No URL configured", null, 0, null)
                : await manager.CheckHttp(service, service, cancellationToken);

            var running = http.IsHealthy;
            var portCheck = false;
            if (!running && service.Port is > 0)
            {
                portCheck = await manager.CheckPort(service, service, cancellationToken);
                running = portCheck;
            }

            service.IsIcmpRunning = manager.CheckICMP(service);
            var wasRunning = service.IsRunning;
            var description = $"{http.Message}; {http.ElapsedMilliseconds} ms";
            if (http.Error != null)
                description += $": {http.Error}";
            if (service.Port is > 0)
                description += portCheck ? "; TCP port reachable" : "; TCP port failed";
            description += service.IsIcmpRunning ? "; ICMP passed" : "; ICMP failed (informational)";

            manager.ApplyCheckResult(service, running, description);
            await manager.SaveChangesAsync(cancellationToken);

            if (wasRunning && !service.IsRunning)
            {
                var history = new ServiceDownHistory
                {
                    ServiceId = service.ServiceId,
                    DateAdded = DateTime.Now,
                    Htmlresponse = http.Message + (http.Error == null ? "" : ": " + http.Error),
                    ResponseTime = TimeSpan.FromMilliseconds(http.ElapsedMilliseconds),
                    IsPingFailed = !service.IsIcmpRunning,
                    IsPortFailed = service.Port.HasValue && !portCheck,
                    IsRssfeedFailed = false
                };
                await manager.AddServiceDownHistoryAsync(history, cancellationToken);
            }
            else if (!wasRunning && service.IsRunning)
            {
                var openHistory = service.ServiceDownHistories
                    .Where(h => h.BackUpDateTime == null)
                    .OrderByDescending(h => h.DateAdded)
                    .FirstOrDefault();
                if (openHistory != null)
                {
                    openHistory.BackUpDateTime = DateTime.Now;
                    await manager.SaveChangesAsync(cancellationToken);
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Checked {ServiceName} ({ServiceId}) in {ElapsedMilliseconds} ms: {IsRunning}",
                service.ServiceName, service.ServiceId, stopwatch.ElapsedMilliseconds, running);
        }
    }
}
