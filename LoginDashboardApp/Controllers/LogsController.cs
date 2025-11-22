using LoginDashboardApp.Data;
using LoginDashboardApp.Models;

namespace LoginDashboardApp.Controllers
{
    public class LogsController
    {
        private readonly ApplicationDbContext _db;

        public LogsController(ApplicationDbContext db)
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
