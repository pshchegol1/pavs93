using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    public class ServicesListModel : PageModel
    {
        private readonly ILogger<ServicesListModel> _logger;
        private readonly Vc3eyeContext _db;
        public List<Service> services { get; set; }
        public List<Service> DeletedServices { get; set; }

        public List<Service> PausedServices { get; set; }
        public List<string> LocationName { get; set; }

        private Thread _ServiceCheckThread = null;

        public ServicesListModel(ILogger<ServicesListModel> logger, Vc3eyeContext db)
        {
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a search term.")]
        public string searchTerm { get; set; }

        [BindProperty]
        public string serviceName { get; set; }



        public void OnGet()
        {
            var sc = new ServiceController(_db);
            services = sc.GetCurrentServices().OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();
            LocationName = sc.GetLocations();


            DeletedServices = sc.GetRemovedServices();
            PausedServices = sc.GetPausedServices();

            // Check if a location parameter was passed in the query string
            if (Request.Query.ContainsKey("location"))
            {
                string? location = Request.Query["location"];

                // Call the OnGetLocation method with the selected location
                OnGetLocation(location);
            }

        }

        public IActionResult OnPostSearch()
        {
            var sc = new ServiceController(_db);
            if (!ModelState.IsValid && searchTerm == null)
            {
                LocationName = sc.GetLocations();
                return Page();
            }



            services = sc.GetServiceBySearch(searchTerm);
            LocationName = sc.GetLocations();
            return Page();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        public void OnGetLocation(string location)
        {
            var sc = new ServiceController(_db);

            services = sc.GetServiceLocation(location).OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public IActionResult OnPostDelete(int serviceid)
        {


            var sc = new ServiceController(_db);
            bool deleted = sc.DeleteServiceById(serviceid);

            if (deleted)
            {
                var lm = new LogsManager(_db);
                var userLog = new Log()
                {
                    LogName = "Delete",
                    LogDescription = "Service - " + serviceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };

                lm.AddUserActivityLog(userLog);

                return RedirectToPage("ServicesList", new { @m = "deleted" });
            }

            return Page();
        }

        /// <summary>
        /// When click CHECK button
        /// </summary>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public async Task <IActionResult> OnPostCheck(int serviceid)
        {
            //initial
            var sc = new ServiceController(_db);
            var sm = new ServiceManager(_db);
            var s = sc.GetServiceByServiceID(serviceid);

            try
            {
                //thread
                Tuple<ServiceManager, Service> obj = new Tuple<ServiceManager, Service>(sm, s);

               await OneCheck(obj);
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

            //reload page
            return RedirectToPage("ServicesList", new { @m = "checked" });
        }

        public async Task<IActionResult> OnPostLookUp(int serviceid)
        {
            //initial
            var sc = new ServiceController(_db);
            var sm = new ServiceManager(_db);
            var s = sc.GetServiceByServiceID(serviceid);

            try
            {
                //thread
                Tuple<ServiceManager, Service> obj = new Tuple<ServiceManager, Service>(sm, s);
                await LookUpCheck(obj);
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

            //reload page
            return RedirectToPage("ServicesList", new { @m = "termchecked" });

        }

        public IActionResult OnPostDeletePermanently(int serviceid)
        {
            if (serviceid > 0)
            {
                var sc = new ServiceController(_db);
                bool deleted = sc.DeleteServicePermanently(serviceid);

                if (deleted)
                {
                    return RedirectToPage("ServicesList", new { @m = "deleted" });
                }
            }

            return RedirectToPage("ServicesList");
        }

        private async Task OneCheck(object obj)
        {
            if (obj is Tuple<ServiceManager, Service> arg)
            {
                ServiceManager sm = arg.Item1;
                Service s = arg.Item2;

                var stc = new SettingContoller(_db);
                Setting? st = null;

                string sDesc = "N/A";

                lock (sm)
                    st = stc.GetCurrentSetting();

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                bool running = false;

                //flag layers
                bool bHTTP = false;
                bool bPort = false;

                if (!string.IsNullOrEmpty(s.Url))
                {
                    lock (sm)
                        bHTTP = running = sm.CheckHTTPStatus(s, s).Result;
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
                        lock (sm)
                            bPort = running = sm.CheckPort(s, s).Result;
                    }
                }

                //set desc string
                if (bPort)
                {
                    sDesc = $"{sDesc} - Port Passed";
                }
                else
                {
                    sDesc = $"{sDesc} - Port Failed";
                }

                //ICMP
                lock (sm)
                    s.IsIcmpRunning = sm.CheckICMP(s);

                //set desc string
                if (s.IsIcmpRunning)
                    sDesc = $"{sDesc} - ICMP Passed";
                else
                    sDesc = $"{sDesc} - ICMP Failed";

                //service s
                s.IsRunning = running;
                s.LastTimeChecked = DateTime.Now;
                s.NotificationMessage = sDesc;
                lock (sm)
                    sm.ModifyService(s);

                //send notification MS Teams webhook
                if (!s.IsRunning && st != null) await sm.MSTeamsNotify(st, s);

                stopwatch.Stop();
                Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds);
            }

            RedirectToPage("ServicesList");
        }
        private async Task LookUpCheck(object obj)
        {
            if (obj is Tuple<ServiceManager, Service> arg)
            {
                //flag layers
                bool bLookUp = false;

                ServiceManager sm = arg.Item1;
                Service s = arg.Item2;

                var stc = new SettingContoller(_db);
                Setting st = null;

                string sDesc = s.NotificationMessage ?? "";

                lock (sm)
                    st = stc.GetCurrentSetting();

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                bool running = false;

                if (!string.IsNullOrEmpty(s.Url) && !string.IsNullOrEmpty(s.LookupTerm))
                {
                    lock (sm)
                        bLookUp = running = sm.CheckHTTPContent(s, s).Result;
                }

                //set desc string
                if (bLookUp)
                {
                    sDesc = $"{sDesc} - Keyword Found";
                }
                else
                {
                    sDesc = $"{sDesc} - Keyword Not Found";
                }

                //service s
                s.IsRunning = running;
                s.LastTimeChecked = DateTime.Now;
                lock (sm)
                    sm.ModifyService(s);

                //send notification MS Teams webhook
                if (!s.IsRunning && st != null) await sm.MSTeamsNotify(st, s);

                stopwatch.Stop();
                Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
