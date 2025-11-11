using VC3EYE.Entities;

namespace VC3EYE.Data
{
    public class ServiceDownHistoryManager
    {
        private readonly Vc3eyeContext _db;

        public ServiceDownHistoryManager(Vc3eyeContext db)
        {
            _db = db;
        }

        public bool AddServiceDownHistory(ServiceDownHistory downHistory)
        {
            bool added = false;

            try
            {
                _db.ServiceDownHistories.Add(downHistory);
                if (_db.SaveChanges() >= 0)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Add Service Down History",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return added;
        }
    }
}
