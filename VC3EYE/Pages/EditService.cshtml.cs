using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Security.Claims;

using VC3EYE.Data;
using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator + "," + SiteRoles.User)]
    public class EditServiceModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public EditServiceModel(Vc3eyeContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ServiceModel Service { get; set; } = new();

        public void OnGet(int serviceid)
        {
            var sc = new ServiceController(_db);
            var service = sc.GetServiceByServiceID(serviceid);

            int minutes = (int)Math.Round(service.CheckInterval.TotalMinutes);

            var serviceM = new ServiceModel()
            {
                ServiceID = service.ServiceId,
                ServiceName = service.ServiceName,
                ServiceURL = service.Url,
                Location = service.Location,
                IPAddress = service.Ipaddress,
                GeneralInformation = service.GeneralInformation,
                IsImportant = service.IsImportant,
                IsActive = service.IsActive,
                Port = service.Port,
                RSSFeedLink = service.RssfeedLink,
                CheckInterval = minutes.ToString(),
                NumOfBackup = service.NumOfBackup,
                IsRunning = service.IsRunning,
                IsInternal = service.IsInternal,
                NotificationMessage = service.NotificationMessage,
                IsDeleted = service.IsDeleted,
                NotifyByMSTeams = service.NotifyByMsteams,
                LookupTerm = service.LookupTerm
            };

            Service = serviceM;
        }

        public IActionResult OnPostEdit(int serviceid)
        {
            if (ModelState.IsValid)
            {
                Service.ServiceID = serviceid;
                var sc = new ServiceController(_db);
                bool updated = sc.ModifyService(Service);

                if (updated)
                {
                    var lm = new LogsManager(_db);
                    var userLog = new Log()
                    {
                        LogName = "Update",
                        LogDescription = "Service - " + Service.ServiceName,
                        IsAudit = false,
                        DateAdded = DateTime.Now,
                        UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                    };

                    lm.AddUserActivityLog(userLog);

                    return RedirectToPage("EditService", new { @serviceid = serviceid, @m = "updated" });
                }
                else
                {
                    return RedirectToPage("EditService", new { @serviceid = serviceid, @m = "upfailed" });
                }
            }

            var serviceEntity = new ServiceController(_db).GetServiceByServiceID(serviceid);
            Service.ServiceID = serviceid;
            if (serviceEntity != null)
            {
                Service.ServiceName = serviceEntity.ServiceName;
                Service.ServiceURL = serviceEntity.Url;
                Service.Location = serviceEntity.Location;
                Service.IPAddress = serviceEntity.Ipaddress;
                Service.GeneralInformation = serviceEntity.GeneralInformation;
                Service.IsImportant = serviceEntity.IsImportant;
                Service.Port = serviceEntity.Port;
                Service.RSSFeedLink = serviceEntity.RssfeedLink;
                Service.IsInternal = serviceEntity.IsInternal;
                Service.NotifyByMSTeams = serviceEntity.NotifyByMsteams;
                Service.NotificationMessage = serviceEntity.NotificationMessage;
                Service.LookupTerm = serviceEntity.LookupTerm;
                Service.NumOfBackup = serviceEntity.NumOfBackup;
                Service.IsDeleted = serviceEntity.IsDeleted;
            }
            return Page();
        }
    }
}
