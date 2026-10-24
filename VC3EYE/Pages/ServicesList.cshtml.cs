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
                if (s == null)
                {
                    TempData["CheckAlertType"] = "danger";
                    TempData["CheckAlertMessage"] = "The selected service could not be found, so its status was not checked.";
                    return RedirectToPage("ServicesList");
                }

                //thread
                Tuple<ServiceManager, Service> obj = new Tuple<ServiceManager, Service>(sm, s);

                await OneCheck(obj);
                SetCheckAlert(s, "Availability check");
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
                TempData["CheckAlertType"] = "danger";
                TempData["CheckAlertMessage"] = "The availability check failed before a service result could be saved.";
            }

            //reload page
            return RedirectToPage("ServicesList");
        }

        public async Task<IActionResult> OnPostLookUp(int serviceid)
        {
            //initial
            var sc = new ServiceController(_db);
            var sm = new ServiceManager(_db);
            var s = sc.GetServiceByServiceID(serviceid);

            try
            {
                if (s == null)
                {
                    TempData["CheckAlertType"] = "danger";
                    TempData["CheckAlertMessage"] = "The selected service could not be found, so its status was not checked.";
                    return RedirectToPage("ServicesList");
                }

                //thread
                Tuple<ServiceManager, Service> obj = new Tuple<ServiceManager, Service>(sm, s);
                await LookUpCheck(obj);
                SetCheckAlert(s, "Page content check");
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
                TempData["CheckAlertType"] = "danger";
                TempData["CheckAlertMessage"] = "The page content check failed before a service result could be saved.";
            }

            //reload page
            return RedirectToPage("ServicesList");

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
                var st = new SettingContoller(_db).GetCurrentSetting();
                var timer = System.Diagnostics.Stopwatch.StartNew();
                var http = await sm.CheckHttp(s, s);
                var passed = http.IsHealthy;
                var diagnostic = $"{http.Message}{(http.Error == null ? "" : ": " + http.Error)}; {http.ElapsedMilliseconds} ms";

                // A configured TCP port is a fallback when the HTTP endpoint is not healthy.
                if (!passed && s.Port is > 0 && await sm.CheckPort(s, s))
                {
                    passed = true;
                    diagnostic += "; TCP port reachable";
                }

                s.IsIcmpRunning = sm.CheckICMP(s);
                diagnostic += s.IsIcmpRunning ? "; ICMP passed" : "; ICMP failed (informational)";
                // A successful manual probe should immediately clear a stale Down
                // state; failures still use the configured consecutive-failure threshold.
                sm.ApplyCheckResult(s, passed, diagnostic, recoveryChecks: 1);
                sm.ModifyService(s);

                if (!s.IsRunning && st != null) await sm.MSTeamsNotify(st, s);
                timer.Stop();
            }

            RedirectToPage("ServicesList");
        }
        private async Task LookUpCheck(object obj)
        {
            if (obj is Tuple<ServiceManager, Service> arg)
            {
                ServiceManager sm = arg.Item1;
                Service s = arg.Item2;
                // This is a content diagnostic only. A missing term (common on
                // JavaScript-rendered or bot-protected pages) must not change the
                // service's availability state or send a false outage notification.
                var termFound = await sm.CheckHTTPContent(s, s);
                var result = await sm.CheckHttp(s, s);
                var isAvailable = result.IsHealthy;
                if (!isAvailable && s.Port is > 0 && await sm.CheckPort(s, s))
                    isAvailable = true;

                var contentStatus = string.IsNullOrWhiteSpace(s.LookupTerm)
                    ? "content check skipped: no lookup term configured"
                    : termFound ? $"content check: '{s.LookupTerm}' found" : $"content check: '{s.LookupTerm}' not found";
                var availabilityStatus = $"availability check: {(isAvailable ? "reachable" : "unreachable")}";
                var diagnostic = $"{contentStatus}; {availabilityStatus}; {result.Message}";
                sm.ApplyCheckResult(s, isAvailable, diagnostic, recoveryChecks: 1);
                sm.ModifyService(s);
            }
        }

        private void SetCheckAlert(Service service, string checkName)
        {
            var status = service.IsRunning ? "RUNNING" : "DOWN";
            var detail = string.IsNullOrWhiteSpace(service.NotificationMessage)
                ? "No diagnostic details are available."
                : service.NotificationMessage;

            TempData["CheckAlertType"] = service.IsRunning ? "success" : "danger";
            TempData["CheckAlertMessage"] = $"{checkName} complete: {service.ServiceName} is {status}. {detail}";
        }
    }
}
