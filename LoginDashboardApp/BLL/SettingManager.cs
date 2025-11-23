using LoginDashboardApp.Data;
using LoginDashboardApp.Models;

namespace LoginDashboardApp.BLL
{
    public class SettingManager
    {
        private readonly ApplicationDbContext _db;
        public SettingManager(ApplicationDbContext db)
        {
            _db = db;
        }

        public Setting GetCurrentSetting()
        {
            Setting s = _db.Settings.FirstOrDefault();

            return s;
        }

        public Setting GetSettingByID(int settingid)
        {
            Setting s = _db.Settings.Where(x => x.SettingId == settingid).FirstOrDefault();

            return s;
        }

        public bool AddSetting(Setting setting)
        {
            bool added = false;

            try
            {
                _db.Settings.Add(setting);

                if (_db.SaveChanges() >= 0)
                {
                    added = true;
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Add setting",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return added;
        }

        public bool UpdateSetting(Setting setting)
        {
            bool updated = false;

            try
            {
                if (_db.SaveChanges() >= 0)
                {
                    updated = true;
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Update setting",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return updated;
        }
    }
}
