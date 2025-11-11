using VC3EYE.Entities;

namespace VC3EYE.Data
{
    public class LogsController
    {
        private readonly Vc3eyeContext _db;

        public LogsController(Vc3eyeContext db) 
        {
            _db = db;
        }

        public List<Log> GetAllLogs()
        {
            var lm = new LogsManager(_db);
            var logs = lm.GetAllLogs();

            return logs;
        }

        public List<ErrorLog> GetAllErrorLogs()
        {
            var lm = new LogsManager(_db);
            var errorLogs = lm.GetAllErrorLogs();

            return errorLogs;
        }
    }
}
