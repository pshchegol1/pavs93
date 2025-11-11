using VC3EYE.Entities;

namespace VC3EYE.Data
{
    public class SettingManager
    {
        //declare database
        private readonly Vc3eyeContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public SettingManager(Vc3eyeContext db)
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
