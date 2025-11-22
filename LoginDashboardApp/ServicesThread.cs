using LoginDashboardApp.Data;
using LoginDashboardApp.Models;

namespace LoginDashboardApp
{
    public class ServicesThread : BackgroundService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ServicesThread> _logger;

        public ServicesThread(ApplicationDbContext db, ILogger<ServicesThread> logger)
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

                bool bHTTP = false;
                string sDesc = "N/A";

                //Stopwatch for testing (Remove later)
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                bool running = false;

                if (!string.IsNullOrEmpty(s.Url))
                {

                    bHTTP = running = sm.CheckHTTPStatus(s, s).Result;
                    HTMLResponseCheck = sm.CheckHTTPResponse(s, s).Result.Item3;
                }

                //set desc string
                if (bHTTP)
                {
                    sDesc = "Checked - Http/Https Passed.";
                }

                if (!running)
                {
                    //set desc string
                    lock (sm)
                        sDesc = $"Checked - {sm.CheckHTTPResponse(s, s).Result.Item2} - {sm.CheckHTTPResponse(s, s).Result.Item3}";

                    if (s.Port != null && s.Port > 0)
                    {

                        running = sm.CheckPort(s, s).Result;
                        PortCheck = running;
                    }
                }

                //set desc string
                if (PortCheck)
                {
                    sDesc = $"{sDesc} - Port Passed";
                }
                else
                {
                    sDesc = $"{sDesc} - Port Failed";
                }

                //ICMP
                if (!string.IsNullOrEmpty(s.Url) || !string.IsNullOrEmpty(s.Ipaddress))
                {
                    s.IsIcmpRunning = sm.CheckICMP(s);
                }

                //set desc string
                if (s.IsIcmpRunning)
                    sDesc = $"{sDesc} - ICMP Passed";
                else
                    sDesc = $"{sDesc} - ICMP Failed";

                s.NotificationMessage = sDesc;

                if (s.IsRunning == true && running == true)
                {
                    s.LastTimeChecked = DateTime.Now;
                    lock (sm)
                        sm.ModifyService(s);
                }
                else
                {
                    if (!running)
                    {
                        if (s.NumOfDownTimes > 0)
                            s.NumOfDownTimes = 0;
                        if (s.NumOfDownTimes >= -s.NumOfBackup)
                            s.NumOfDownTimes--;
                        if (s.NumOfDownTimes == -s.NumOfBackup)
                        {
                            //Setting down if the service has passed the alloted checks
                            //service = s
                            s.IsRunning = running;
                            s.LastTimeChecked = DateTime.Now;
                            lock (sm)
                                sm.ModifyService(s);

                            var sdh = new ServiceDownHistoryManager(_db);
                            ServiceDownHistory downHistory = new ServiceDownHistory()
                            {
                                ServiceId = s.ServiceId,
                                DateAdded = DateTime.Now,
                                Htmlresponse = HTMLResponseCheck,
                                ResponseTime = null,
                                IsPingFailed = PingCheck,
                                IsPortFailed = PortCheck,
                                IsRssfeedFailed = RSSCheck
                            };
                            lock (sdh)
                                sdh.AddServiceDownHistory(downHistory);

                        }
                        else
                        {
                            s.LastTimeChecked = DateTime.Now;
                            s.IsRunning = running;
                            lock (sm)
                                sm.ModifyService(s);
                        }

                    }
                    else
                    {
                        if (s.NumOfDownTimes < 0)
                            s.NumOfDownTimes = 0;
                        s.NumOfDownTimes++;
                        if (s.NumOfDownTimes == 3)
                        {
                            //Setting the service as running if it passes 3 susseccful checks
                            //service = s
                            s.IsRunning = running;
                            s.LastTimeChecked = DateTime.Now;
                            if (s.ServiceDownHistories.Count > 0)
                                s.ServiceDownHistories.Last().BackUpDateTime = DateTime.Now;
                            lock (sm)
                                sm.ModifyService(s);
                        }
                        else
                        {
                            s.LastTimeChecked = DateTime.Now;
                            s.IsRunning = running;
                            lock (sm)
                                sm.ModifyService(s);
                        }
                    }

                }

                stopwatch.Stop();
                Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + " " + running + " " + s.ServiceName);
            }
        }
    }
}
