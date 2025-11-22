using LoginDashboardApp.Data;
using LoginDashboardApp.Models;

namespace LoginDashboardApp.Controllers
{
    public class SettingContoller
    {
        private readonly ApplicationDbContext _db;

        public SettingContoller(ApplicationDbContext db)
        {
            _db = db;
        }

        public Setting GetCurrentSetting()
        {
            SettingManager sm = new SettingManager(_db);
            return sm.GetCurrentSetting();
        }

        public bool AddSetting(Setting setting)
        {
            var sm = new SettingManager(_db);
            bool added = false;

            return added;
        }

        public bool UpdateSetting(Setting setting)
        {
            var sm = new SettingManager(_db);
            bool updated = sm.UpdateSetting(setting);

            return updated;
        }
    }
}
