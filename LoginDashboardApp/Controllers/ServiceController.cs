using LoginDashboardApp.Models;
using LoginDashboardApp.Data;
using LoginDashboardApp.BLL;

namespace LoginDashboardApp.Controllers
{
    public class ServiceController
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public ServiceController(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Service> GetCurrentServices()
        {
            ServiceManager serviceManager = new ServiceManager(_db);
            return serviceManager.GetAllServices();
        }
    }
}
