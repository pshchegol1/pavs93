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
    public class AddServiceModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public string? Message { get; set; }

        public AddServiceModel(Vc3eyeContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ServiceModel currentService { get; set; } = new();

        /// <summary>
        /// When page load
        /// </summary>
        public void OnGet()
        {
            Message = null;
        }

        /// <summary>
        /// Action when click ADD
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostAdd()
        {
            if (string.IsNullOrWhiteSpace(currentService.ServiceURL) && string.IsNullOrWhiteSpace(currentService.IPAddress))
            {
                ModelState.AddModelError(string.Empty, "Enter a service URL or IP address so VC3EYE knows what to monitor.");
            }

            if (ModelState.IsValid)
            {
                //declare
                var sc = new ServiceController(_db);
                currentService.UserID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                //flag
                bool confirmation = sc.AddService(currentService);

                if (!confirmation)
                {
                    ModelState.AddModelError(string.Empty, "The service could not be saved. Please review the details and try again.");
                    return Page();
                }

                var lm = new LogsManager(_db);
                var userLog = new Log()
                {
                    LogName = "Add",
                    LogDescription = "Service - " + currentService.ServiceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = currentService.UserID
                };

                //logging
                lm.AddUserActivityLog(userLog);

                //message log
                //go to Service List
                return RedirectToPage("ServicesList");
            }

            return Page();
        }


        /// <summary>
        /// Action when hit RESET
        /// </summary>
        public IActionResult OnPostGoBack()
        {
            ModelState.Clear(); // clear any validation errors

            //reload page
            return RedirectToPage("ServicesList");
        }
    }
}
