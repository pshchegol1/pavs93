using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<Service> services { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly Vc3eyeContext _db;

        [BindProperty]
        [Required(ErrorMessage = "Please enter a search term.")]
        public string search { get; set; }
        public bool IsFiltered { get; set; }
        public List<string> Locations { get; set; }

        public IndexModel(ILogger<IndexModel> logger, Vc3eyeContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet(string location)
        {
            var serv = new ServiceController(_db);
            Locations = serv.GetLocations();

            if (string.IsNullOrEmpty(location))
            {
                services = serv.GetCurrentServices().OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();
            }
            else
            {
                services = serv.GetServiceLocation(location).OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();
            }
        }

        public IActionResult OnPostSearch()
        {
            var sc = new ServiceController(_db);
            Locations = sc.GetLocations();

            if (!ModelState.IsValid)
            {
                services = sc.GetCurrentServices().OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList(); 
                search = "";
                return Page();
            }
           

            services = sc.GetServiceBySearch(search).OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();

            IsFiltered = true;

            search = "";

            return Page();
        }

        
    }
}