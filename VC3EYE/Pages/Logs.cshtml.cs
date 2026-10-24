using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator + "," + SiteRoles.User)]
    public class LogsModel : PageModel
    {
        private const int PageSize = 20;
        private readonly Vc3eyeContext _db;

        public List<Log> SiteLogs { get; private set; } = new();
        public int NumOfPages { get; private set; }
        public int TotalLogs { get; private set; }
        public int MatchingLogs { get; private set; }
        public int AuditLogs { get; private set; }
        public int CurrentPage { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public LogsModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync(int p = 1, CancellationToken cancellationToken = default)
        {
            TotalLogs = await _db.Logs.CountAsync(cancellationToken);
            AuditLogs = await _db.Logs.CountAsync(log => log.IsAudit, cancellationToken);

            var query = _db.Logs.AsNoTracking().Include(log => log.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                query = query.Where(log => log.LogName.Contains(term) ||
                    log.LogDescription.Contains(term) ||
                    log.User.FirstName.Contains(term) ||
                    log.User.LastName.Contains(term) ||
                    (log.User.FirstName + " " + log.User.LastName).Contains(term));
            }

            MatchingLogs = await query.CountAsync(cancellationToken);
            NumOfPages = (int)Math.Ceiling(MatchingLogs / (double)PageSize);
            CurrentPage = NumOfPages == 0 ? 1 : Math.Clamp(p, 1, NumOfPages);

            SiteLogs = await query
                .OrderByDescending(log => log.DateAdded)
                .ThenByDescending(log => log.LogId)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
