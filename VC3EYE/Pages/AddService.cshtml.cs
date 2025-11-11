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

        public string Message { get; set; }

        public AddServiceModel(Vc3eyeContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ServiceModel currentService { get; set; }

        /// <summary>
        /// When page load
        /// </summary>
        public void OnGet()
        {
            //preload
            Message = "Please enter the service URL or IP for checking!";
        }

        /// <summary>
        /// Action when click ADD
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostAdd()
        {
            //process if input is valid
            if (ModelState.IsValid)
            {
                //check one of required fields in input and set feedback
                if (currentService.ServiceURL == null && currentService.IPAddress == null)
                {
                    Message = "Please enter the service URL or IP for checking!";

                    //pop up 2
                    string script = "<script>showMessage();</script>";
                    ViewData["Script"] = script;

                    return Page();
                }

                //declare
                var sc = new ServiceController(_db);
                currentService.UserID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

                //flag
                bool confirmation = sc.AddService(currentService);

                var lm = new LogsManager(_db);
                var userLog = new Log()
                {
                    LogName = "Add",
                    LogDescription = "Service - " + currentService.ServiceName,
                    IsAudit = false,
                    DateAdded = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };

                //logging
                lm.AddUserActivityLog(userLog);

                //message log
                Message = "Service added successful!";

                //go to Service List
                return RedirectToPage("ServicesList");
            }

            // Add logic
            return Page();
        }


        /// <summary>
        /// Action when hit RESET
        /// </summary>
        public IActionResult OnPostReset()
        {
            //call reset value function
            //InitPage();

            ModelState.Clear(); // clear any validation errors

            // Add logic
            return Page();
        }


        public IActionResult OnPostGoBack()
        {
            ModelState.Clear(); // clear any validation errors

            //reload page
            return RedirectToPage("ServicesList");
        }
    }
}
