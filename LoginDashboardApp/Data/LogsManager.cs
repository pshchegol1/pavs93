using LoginDashboardApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginDashboardApp.Data
{
    public class LogsManager
    {
        private readonly ApplicationDbContext _db;

        public LogsManager(ApplicationDbContext db) { _db = db; }

        public bool AddUserActivityLog(Log log)
        {
            bool added = false;

            try
            {
                _db.Logs.Add(log);
                if (_db.SaveChanges() >= 0)
                {
                    added = true;
                }
            }
            catch (Exception ex)
            {
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Add User Activity Log",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                AddErrorLog(errLog);
            }

            return added;
        }

        public bool AddErrorLog(ErrorLog errorLog)
        {
            bool added = false;

            try
            {
                _db.ErrorLogs.Add(errorLog);
                if (_db.SaveChanges() >= 0)
                {
                    added = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Critical Error: Failed to add error log to the database.", ex);
            }

            return added;
        }

        public List<Log> GetAllLogs()
        {
            var logs = _db.Logs.Include(x => x.User).ToList();

            return logs;
        }

        public List<ErrorLog> GetAllErrorLogs()
        {
            var errlogs = _db.ErrorLogs.OrderByDescending(x => x.DateAdded).ToList();

            return errlogs;
        }

    }
}
