using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using LoginDashboardApp.Models;
using LoginDashboardApp.Data;
using LoginDashboardApp.Controllers;

namespace LoginDashboardApp.Views.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<Service> services {  get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }


        public void OnGet()
        {
            var serv = new ServiceController(_db);
            services = serv.GetCurrentServices().OrderBy(service => service.IsRunning).ThenBy(service => !service.IsImportant).ToList();
        }


    }

}
