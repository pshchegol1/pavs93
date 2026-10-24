using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Data;
using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator)]
    public class SettingsModel : PageModel
    {
        private readonly Vc3eyeContext _db;
        private readonly ILogger<SettingsModel> _logger;

        [BindProperty]
        public VC3EYE.Models.SettingsModel Setting { get; set; } = new();

        public bool HasSmtpPassword { get; private set; }
        public bool HasTeamsSecret { get; private set; }

        public SettingsModel(Vc3eyeContext db, ILogger<SettingsModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var current = await _db.Settings.AsNoTracking()
                .OrderBy(setting => setting.SettingId)
                .FirstOrDefaultAsync(cancellationToken);

            if (current != null)
            {
                Setting.SettingId = current.SettingId;
                Setting.SmtpPort = current.SmtpPort;
                Setting.SmtpUserName = current.SmtpUserName;
                Setting.MtClientId = current.MtClientId;
                HasSmtpPassword = !string.IsNullOrWhiteSpace(current.SmtpPassword);
                HasTeamsSecret = !string.IsNullOrWhiteSpace(current.MtSecretKey);
            }
        }

        public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                Setting.SmtpPassword = null;
                Setting.MtSecretKey = null;
                ModelState.Remove("Setting.SmtpPassword");
                ModelState.Remove("Setting.MtSecretKey");
                await LoadSecretFlagsAsync(cancellationToken);
                return Page();
            }

            try
            {
                var current = Setting.SettingId > 0
                    ? await _db.Settings.FirstOrDefaultAsync(setting => setting.SettingId == Setting.SettingId, cancellationToken)
                    : await _db.Settings.OrderBy(setting => setting.SettingId).FirstOrDefaultAsync(cancellationToken);

                if (current == null)
                {
                    current = new Setting { DateAdded = DateTime.Now };
                    _db.Settings.Add(current);
                }

                current.SmtpPort = Setting.SmtpPort;
                current.SmtpUserName = Setting.SmtpUserName;
                current.MtClientId = Setting.MtClientId;

                if (Setting.ClearSmtpPassword)
                    current.SmtpPassword = null;
                else if (!string.IsNullOrWhiteSpace(Setting.SmtpPassword))
                    current.SmtpPassword = Setting.SmtpPassword;

                if (Setting.ClearTeamsSecret)
                    current.MtSecretKey = null;
                else if (!string.IsNullOrWhiteSpace(Setting.MtSecretKey))
                    current.MtSecretKey = Setting.MtSecretKey;

                await _db.SaveChangesAsync(cancellationToken);
                TempData["SettingsSaved"] = true;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save application settings");
                Setting.SmtpPassword = null;
                Setting.MtSecretKey = null;
                ModelState.Remove("Setting.SmtpPassword");
                ModelState.Remove("Setting.MtSecretKey");
                ModelState.AddModelError(string.Empty, "Settings could not be saved. Please try again.");
                await LoadSecretFlagsAsync(cancellationToken);
                return Page();
            }
        }

        private async Task LoadSecretFlagsAsync(CancellationToken cancellationToken)
        {
            var current = Setting.SettingId > 0
                ? await _db.Settings.AsNoTracking().FirstOrDefaultAsync(setting => setting.SettingId == Setting.SettingId, cancellationToken)
                : await _db.Settings.AsNoTracking().OrderBy(setting => setting.SettingId).FirstOrDefaultAsync(cancellationToken);

            HasSmtpPassword = !string.IsNullOrWhiteSpace(current?.SmtpPassword);
            HasTeamsSecret = !string.IsNullOrWhiteSpace(current?.MtSecretKey);
        }
    }
}
