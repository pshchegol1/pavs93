using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE
{
    public class ServicesThread : BackgroundService
    {
        private readonly Vc3eyeContext _db;
        private readonly ILogger<ServicesThread> _logger;

        public ServicesThread(Vc3eyeContext db, ILogger<ServicesThread> logger)
        {
            _db = db;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("ServicesThread is running at: {time}", DateTimeOffset.Now);
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    CheckServices();

                    stopwatch.Stop();
                    Console.WriteLine("Check services: " + stopwatch.ElapsedMilliseconds);
                    stopwatch.Reset();
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to ExecuteAsync in Services Thread",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }
        }
        private Thread _ServiceCheckThread = null;

        private void CheckServices()
        {
            try
            {
                var sm = new ServiceManager(_db);
                var services = sm.GetAllServices();

                if (services.Count() > 0)
                {
                    foreach (var s in services)
                    {
                        if (s.LastTimeChecked.AddMinutes(s.CheckInterval.TotalMinutes) < DateTime.Now)
                        {
                            Tuple<ServiceManager, Service> obj = new Tuple<ServiceManager, Service>(sm, s);
                            _ServiceCheckThread = new Thread(new ParameterizedThreadStart(Checks));
                            _ServiceCheckThread.IsBackground = true;
                            _ServiceCheckThread.Start(obj);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Check Services in Services Thread",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }
        }

        private void Checks(object obj)
        {
            if (obj is Tuple<ServiceManager, Service> arg)
            {
                ServiceManager sm = arg.Item1;
                Service s = arg.Item2;

                string HTMLResponseCheck = "";
                bool PortCheck = false;
                bool RSSCheck = false;
                bool PingCheck = false;

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var http = string.IsNullOrWhiteSpace(s.Url)
                    ? new HttpCheckResult(false, null, "No URL configured", null, 0, null)
                    : sm.CheckHttp(s, s).GetAwaiter().GetResult();
                HTMLResponseCheck = http.Message + (http.Error == null ? "" : ": " + http.Error);
                bool running = http.IsHealthy;
                string sDesc = $"{http.Message}; {http.ElapsedMilliseconds} ms";

                if (!running && s.Port is > 0)
                {
                    PortCheck = sm.CheckPort(s, s).GetAwaiter().GetResult();
                    running = PortCheck;
                    sDesc += PortCheck ? "; TCP port reachable" : "; TCP port failed";
                }

                //ICMP
                if (!string.IsNullOrEmpty(s.Url) || !string.IsNullOrEmpty(s.Ipaddress))
                {
                    s.IsIcmpRunning = sm.CheckICMP(s);
                }
                PingCheck = s.IsIcmpRunning;

                //set desc string
                if (s.IsIcmpRunning)
                    sDesc = $"{sDesc}; ICMP passed";
                else
                    sDesc = $"{sDesc}; ICMP failed (informational)";

                var wasRunning = s.IsRunning;
                sm.ApplyCheckResult(s, running, sDesc);
                lock (sm) sm.ModifyService(s);
                if (wasRunning && !s.IsRunning)
                {
                    var sdh = new ServiceDownHistoryManager(_db);
                    sdh.AddServiceDownHistory(new ServiceDownHistory { ServiceId = s.ServiceId, DateAdded = DateTime.Now,
                        Htmlresponse = HTMLResponseCheck, ResponseTime = TimeSpan.FromMilliseconds(http.ElapsedMilliseconds),
                        IsPingFailed = !PingCheck, IsPortFailed = s.Port.HasValue && !PortCheck, IsRssfeedFailed = RSSCheck });
                }
                else if (!wasRunning && s.IsRunning && s.ServiceDownHistories.Count > 0)
                {
                    s.ServiceDownHistories.Last().BackUpDateTime = DateTime.Now;
                    lock (sm) sm.ModifyService(s);
                }

                stopwatch.Stop();
                Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + " " + running + " " + s.ServiceName);
            }
        }

    }
}
