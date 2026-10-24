using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;

namespace VC3EYE.Pages
{
    public class ChartsofservicesModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public ChartsofservicesModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public int TotalServices { get; private set; }
        public int RunningServices { get; private set; }
        public int DownServices { get; private set; }
        public int TotalOutagesLast30Days { get; private set; }
        public int PeakDailyOutages { get; private set; }
        public List<DailyOutagePoint> DailyOutages { get; private set; } = new();
        public List<ServiceOutageSummary> MostOutages { get; private set; } = new();
        public List<RecentOutage> RecentOutages { get; private set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var activeServices = await _db.Services
                .Where(service => service.IsActive && !service.IsDeleted)
                .Select(service => new { service.ServiceId, service.ServiceName, service.IsRunning })
                .ToListAsync(cancellationToken);

            TotalServices = activeServices.Count;
            RunningServices = activeServices.Count(service => service.IsRunning);
            DownServices = TotalServices - RunningServices;

            var startDate = DateTime.Today.AddDays(-29);
            var history = await _db.ServiceDownHistories
                .Include(outage => outage.Service)
                .Where(outage => outage.DateAdded >= startDate)
                .OrderByDescending(outage => outage.DateAdded)
                .ToListAsync(cancellationToken);

            TotalOutagesLast30Days = history.Count;
            var outagesByDay = history.GroupBy(outage => outage.DateAdded.Date)
                .ToDictionary(group => group.Key, group => group.Count());

            DailyOutages = Enumerable.Range(0, 30)
                .Select(offset => startDate.AddDays(offset))
                .Select(day => new DailyOutagePoint(day, outagesByDay.GetValueOrDefault(day.Date)))
                .ToList();
            PeakDailyOutages = DailyOutages.Max(point => point.Count);

            MostOutages = history
                .GroupBy(outage => new { outage.ServiceId, ServiceName = outage.Service?.ServiceName ?? $"Service #{outage.ServiceId}" })
                .Select(group => new ServiceOutageSummary(group.Key.ServiceId, group.Key.ServiceName, group.Count()))
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.ServiceName)
                .Take(6)
                .ToList();

            RecentOutages = history.Take(8)
                .Select(outage => new RecentOutage(
                    outage.ServiceId,
                    outage.Service?.ServiceName ?? $"Service #{outage.ServiceId}",
                    outage.DateAdded,
                    outage.BackUpDateTime,
                    outage.ResponseTime))
                .ToList();
        }
    }

    public sealed record DailyOutagePoint(DateTime Date, int Count);
    public sealed record ServiceOutageSummary(int ServiceId, string ServiceName, int Count);
    public sealed record RecentOutage(int ServiceId, string ServiceName, DateTime Started, DateTime? Recovered, TimeSpan? ResponseTime);
}
