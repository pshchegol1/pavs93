using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator)]
    public class ErrorLogsModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public ErrorLogsModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<ErrorLog> ErrorLogs { get; set; }
        public int TotalErrorLogs { get; set; }

        public void OnGet()
        {
            var lc = new LogsController(_db);
            TotalErrorLogs = lc.GetAllErrorLogs().Count;
            ErrorLogs = lc.GetAllErrorLogs();
        }
    }
}
