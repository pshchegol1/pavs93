using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator + "," + SiteRoles.User)]
    public class LogsModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public LogsModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<Log> SiteLogs { get; set; }
        public int NumOfPages { get; set; }
        public int TotalLogs { get; set; }

        public void OnGet(int p)
        {
            var lc = new LogsController(_db);

            int perPage = 15;
            int totalLogs = lc.GetAllLogs().Count;
            TotalLogs = totalLogs;
            int numOfPages = (int)Math.Ceiling((double)totalLogs / perPage);

            NumOfPages = numOfPages;

            SiteLogs = lc.GetAllLogs().OrderByDescending(x => x.DateAdded).Skip((p - 1) * perPage).Take(perPage).ToList();          
        }
    }
}
