using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator)]
    public class ErrorLogsModel : PageModel
    {
        private const int PageSize = 25;
        private readonly Vc3eyeContext _db;

        public ErrorLogsModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<ErrorLog> ErrorLogs { get; private set; } = new();
        public int TotalErrorLogs { get; private set; }
        public int MatchingErrorLogs { get; private set; }
        public int ArchivedErrorLogs { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ShowArchived { get; set; }

        public async Task OnGetAsync(int p = 1, CancellationToken cancellationToken = default)
        {
            var allLogs = _db.ErrorLogs.AsNoTracking();
            TotalErrorLogs = await allLogs.CountAsync(cancellationToken);
            ArchivedErrorLogs = await allLogs.CountAsync(log => log.IsArchived, cancellationToken);

            var query = allLogs.Where(log => log.IsArchived == ShowArchived);
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                query = query.Where(log => log.ErrorName.Contains(term) || log.ErrorDescription.Contains(term));
            }

            MatchingErrorLogs = await query.CountAsync(cancellationToken);
            TotalPages = (int)Math.Ceiling(MatchingErrorLogs / (double)PageSize);
            CurrentPage = TotalPages == 0 ? 1 : Math.Clamp(p, 1, TotalPages);

            ErrorLogs = await query.OrderByDescending(log => log.DateAdded)
                .ThenByDescending(log => log.ErrorLogId)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
