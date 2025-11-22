using LoginDashboardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoginDashboardApp.Controllers
{
    [Authorize(Policy = "Authenticated")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
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
                }
            };

            return View(viewModel);
        }
    }
}