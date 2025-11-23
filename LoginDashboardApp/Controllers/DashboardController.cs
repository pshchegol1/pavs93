using LoginDashboardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoginDashboardApp.Data;
using LoginDashboardApp.BLL;
namespace LoginDashboardApp.Controllers
{
    [Authorize(Policy = "Authenticated")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var services = new ServiceController(_db).GetCurrentServices();

            var serviceModels = services.Select(s => new ServiceModel
            {
                ServiceID = s.ServiceId,
                ServiceName = s.ServiceName,
                ServiceURL = s.Url,
                Location = s.Location,
                IPAddress = s.Ipaddress,
                GeneralInformation = s.GeneralInformation,
                IsImportant = s.IsImportant,
                IsActive = s.IsActive,
                Port = s.Port,
                RSSFeedLink = s.RssfeedLink,
                CheckInterval = s.CheckInterval.ToString(),
                NumOfDownTimes = s.NumOfDownTimes,
                NumOfBackup = s.NumOfBackup,
                DateAdded = s.DateAdded,
                LastDateModified = s.LastDateModified,
                LastTimeChecked = s.LastTimeChecked,
                Status = s.IsActive ? "Active" : "Inactive",
                IsInternal = s.IsInternal,
                IsChecked = false, // Set as needed
                UserID = s.UserId,
                IsRunning = s.IsRunning,
                NotificationMessage = s.NotificationMessage,
                IsDeleted = s.IsDeleted,
                NotifyByMSTeams = s.NotifyByMsteams,
                LookupTerm = s.LookupTerm
            }).ToList();

            var viewModel = new DashboardViewModel
            {
                UserName = User.Identity?.Name ?? "User",
                UserEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                LoginTime = DateTime.Now,
                RecentActivities = new List<string>
                {
                    "Updated project documentation",
                    "Completed Q3 financial report",
                    "Added new team members",
                    "Deployed new features to production",
                    "Reviewed customer feedback"
                },
                Services = serviceModels
            };

            return View(viewModel);
        }
    }
}