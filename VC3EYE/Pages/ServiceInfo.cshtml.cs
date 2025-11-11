using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using VC3EYE.Data;
using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Pages
{
    public class ServiceInfoModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public ServiceInfoModel(Vc3eyeContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Service ServiceDetails { get; set; }     

        public void OnGet(int serviceid)
        {
            if(serviceid > 0)
            {
                var sc = new ServiceController(_db);
                ServiceDetails = sc.GetServiceByServiceID(serviceid);
            }
            else
            {
                //TO DO: redirect to error page
            }
        }

        public IActionResult OnPostEditSatus(int serviceid) 
        {
            if(serviceid > 0)
            {
                ServiceDetails.ServiceId = serviceid;
                var sc = new ServiceController(_db);
                bool updated = sc.EditServiceStatus(ServiceDetails);

                if (updated)
                {
                    var lm = new LogsManager(_db);
                    var userLog = new Log()
                    {
                        LogName = "Edit Service Status",
                        LogDescription = "Service - " + ServiceDetails.ServiceName,
                        IsAudit = false,
                        DateAdded = DateTime.Now,
                        UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                    };

                    lm.AddUserActivityLog(userLog);

                    return RedirectToPage("ServiceInfo", new { @serviceid = serviceid,  @m = "updated" });
                }
            }

            return Page();
        }

        public IActionResult OnPostPauseService(int serviceid)
        {          
            var sc = new ServiceController(_db);
            bool paused = sc.PauseService(serviceid);
            var lm = new LogsManager(_db);

            if (paused)
            {              
                var userLog = new Log()
                {
                    LogName = "Pause Service",
                    LogDescription = "Service - " + ServiceDetails.ServiceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };

                lm.AddUserActivityLog(userLog);

                return RedirectToPage("ServiceInfo", new { @serviceid = serviceid});
            }
            else if(!paused)
            {
                var userLog = new Log()
                {
                    LogName = "Resume Service",
                    LogDescription = "Service - " + ServiceDetails.ServiceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };

                lm.AddUserActivityLog(userLog);

                return RedirectToPage("ServiceInfo", new { @serviceid = serviceid });
            }

            return Page();
        }

        public IActionResult OnPostUndoDelete(int serviceid)
        {
            var sc = new ServiceController(_db);
            bool success = sc.UndoDelete(serviceid);

            if (success)
            {
                var lm = new LogsManager(_db);
                var userLog = new Log()
                {
                    LogName = "Undo Delete",
                    LogDescription = "Service - " + ServiceDetails.ServiceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };

                lm.AddUserActivityLog(userLog);

                return RedirectToPage("ServiceInfo", new { @serviceid = serviceid });
            }

            return Page();
        }
    }
}
