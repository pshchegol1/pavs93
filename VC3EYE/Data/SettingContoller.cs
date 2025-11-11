using VC3EYE.Entities;

namespace VC3EYE.Data
{
    public class SettingContoller
    {
        //initial
        private readonly Vc3eyeContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public SettingContoller(Vc3eyeContext db)
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
