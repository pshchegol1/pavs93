using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator)]
    public class ViewUsersModel : PageModel
    {
        private readonly Vc3eyeContext _db;

        public List<UserDirectoryEntry> Users { get; private set; } = new();
        public int TotalUsers { get; private set; }
        public int AdminCount { get; private set; }
        public int UserCount { get; private set; }
        public int ViewerCount { get; private set; }

        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public ViewUsersModel(Vc3eyeContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var allUsers = await _db.Users
                .AsNoTracking()
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .Select(user => new UserDirectoryEntry(
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.City,
                    user.ProvinceState,
                    user.Role.Description,
                    user.Services.Count))
                .ToListAsync(cancellationToken);

            TotalUsers = allUsers.Count;
            AdminCount = allUsers.Count(user => string.Equals(user.Role, SiteRoles.Administrator, StringComparison.OrdinalIgnoreCase));
            UserCount = allUsers.Count(user => string.Equals(user.Role, SiteRoles.User, StringComparison.OrdinalIgnoreCase));
            ViewerCount = allUsers.Count(user => string.Equals(user.Role, SiteRoles.Viewer, StringComparison.OrdinalIgnoreCase));

            var query = allUsers.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                query = query.Where(user =>
                    $"{user.FirstName} {user.LastName}".Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (user.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    user.City.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            Users = query.ToList();
        }
    }

    public sealed record UserDirectoryEntry(
        int UserId,
        string FirstName,
        string LastName,
        string? Email,
        string City,
        string ProvinceState,
        string Role,
        int ServiceCount);
}
