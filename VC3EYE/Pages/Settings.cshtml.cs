using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VC3EYE.Data;
using VC3EYE.Models;
using VC3EYE.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace VC3EYE.Pages
{
    [Authorize(Roles = SiteRoles.Administrator)]
    public class SettingsModel : PageModel
    {
        private readonly Vc3eyeContext _db;
        public string Message { get; set; }

        [BindProperty]
        public Setting Setting { get; set; }
        public SettingsModel(Vc3eyeContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            var sc = new SettingContoller(_db);

            Setting = sc.GetCurrentSetting();
        }
        public async Task<IActionResult> OnPostSave(int settingid)
        {
            if(settingid > 0)
            {
                var sm = new SettingManager(_db);
                var sc = new SettingContoller(_db);
                var s = sm.GetSettingByID(settingid);

                if(s != null)
                {
                    s.SmtpPort = Setting.SmtpPort;
                    s.SmtpPassword = Setting.SmtpPassword;
                    s.MtClientId = Setting.MtClientId;
                    s.MtSecretKey = Setting.MtSecretKey;

                    sc.UpdateSetting(s);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    await _db.Settings.AddAsync(Setting);
                    await _db.SaveChangesAsync();
                    Message = "Successfully Added Settings";
                    //return RedirectToPage("Services");
                }
            }
            

            return Page();
        }
    }
}
