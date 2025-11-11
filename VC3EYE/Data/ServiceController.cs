using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Data
{
    /// <summary>
    /// Controller for processing the Service
    /// </summary>
    public class ServiceController
    {
        //initial
        private readonly Vc3eyeContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public ServiceController(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<Service> GetCurrentServices()
        {
            ServiceManager serviceManager = new ServiceManager(_db);
            return serviceManager.GetAllServices();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool AddService(ServiceModel service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            Service s = new Service()
            {
                ServiceName = service.ServiceName ?? "",
                Url = service.ServiceURL ?? "",
                Location = service.Location ?? "",
                Ipaddress = service.IPAddress,
                GeneralInformation = service.GeneralInformation,
                IsImportant = service.IsImportant,
                IsActive = true,
                Port = service.Port,
                RssfeedLink = service.RSSFeedLink,
                CheckInterval = TimeSpan.FromMinutes(Convert.ToInt32(service.CheckInterval)),
                NumOfDownTimes = service.NumOfDownTimes,
                NumOfBackup = service.NumOfBackup,
                DateAdded = DateTime.Now,
                LastDateModified = DateTime.Now,
                LastTimeChecked = DateTime.Now,
                UserId = service.UserID,
                IsRunning = true,
                IsDeleted = false,
                LookupTerm = service.LookupTerm
            };

            return serviceManager.AddService(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool ModifyService(ServiceModel serviceModel)
        {
            ServiceManager sm = new ServiceManager(_db);

            var service = sm.GetServiceByID(serviceModel.ServiceID);
            bool updated = false;

            try
            {
                if (service != null)
                {
                    service.ServiceName = serviceModel.ServiceName ?? "";
                    service.Url = serviceModel.ServiceURL;
                    service.Location = serviceModel.Location ?? "";
                    service.Ipaddress = serviceModel.IPAddress;
                    service.GeneralInformation = serviceModel.GeneralInformation;
                    service.Port = serviceModel.Port;
                    service.RssfeedLink = serviceModel.RSSFeedLink;
                    service.CheckInterval = TimeSpan.FromMinutes(Convert.ToInt32(serviceModel.CheckInterval));
                    service.NumOfBackup = serviceModel.NumOfBackup;
                    service.IsImportant = serviceModel.IsImportant;
                    service.IsInternal = serviceModel.IsInternal;
                    service.LastDateModified = DateTime.Now;
                    service.NotificationMessage = serviceModel.NotificationMessage;
                    service.NotifyByMsteams = serviceModel.NotifyByMSTeams;
                    service.LookupTerm = serviceModel.LookupTerm;

                    updated = sm.ModifyService(service);
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Modify Service",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return updated;
        }

        public bool UpdateOnCheck(Service service)
        {
            ServiceManager sm = new ServiceManager(_db);
            bool updated = false;

            if (service != null)
            {
                updated = sm.ModifyService(service);
            }

            return updated;
        }

        public bool CheckHTTPStatus(Service service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.CheckHTTPStatus(service, service).Result;
        }

        public Tuple<bool, int, string> CheckHTTPResponse(Service service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.CheckHTTPResponse(service, service).Result;
        }

        public bool CheckPing(Service service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.CheckPing(service, service).Result;
        }

        public bool CheckPort(Service service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.CheckPort(service, service).Result;
        }

        public bool CheckICMP(Service service)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.CheckICMP(service);
        }

        public bool SendTeamsMessage(Setting sm, Service s)
        {
            ServiceManager serviceManager = new ServiceManager(_db);

            return serviceManager.MSTeamsNotify(sm, s).Result;
        }

        public Service? GetServiceByServiceID(int serviceid)
        {
            var sm = new ServiceManager(_db);
            var service = sm.GetServiceByID(serviceid);
            return service;
        }

        public List<Service> GetServiceBySearch(string searchTerm)
        {
            var sm = new ServiceManager(_db);
            var service = sm.GetServiceBySearchTerm(searchTerm);
            return service;
        }

        public List<Service> GetRemovedServices()
        {
            var sm = new ServiceManager(_db);
            var removed = sm.GetDeletedServices();
            return removed;
        }

        public List<Service> GetServiceLocation(string LocVal)
        {
            var sm = new ServiceManager(_db);
            var service = sm.GetServiceByLocation(LocVal);
            return service;
        }

        public List<string> GetLocations()
        {
            var sm = new ServiceManager(_db);
            var loc = sm.GetAllLocations();
            return loc;
        }

        public bool EditServiceStatus(Service service)
        {
            var sm = new ServiceManager(_db);
            var ser = sm.GetServiceByID(service.ServiceId);
            bool updated = false;

            if (ser != null)
            {
                if (ser.IsRunning != service.IsRunning)
                    ser.NumOfDownTimes = 0;
                ser.IsRunning = service.IsRunning;
                ser.NotificationMessage = service.NotificationMessage;


                updated = sm.UpdateServiceStatus(ser);
            }

            return updated;
        }

        public List<Service> GetPausedServices()
        {
            var sm = new ServiceManager(_db);
            var service = sm.GetAllPausedServices();
            return service;
        }

        public bool PauseService(int serviceid)
        {
            var sm = new ServiceManager(_db);
            var ser = sm.GetServiceByID(serviceid);
            bool paused = false;

            if (ser != null && ser.IsActive && !ser.IsDeleted)
            {
                ser.IsActive = false;
                ser.LastDateModified = DateTime.Now;
                paused = sm.PauseService(ser);
            }
            else if (ser != null && !ser.IsActive && !ser.IsDeleted)
            {
                ser.IsActive = true;
                ser.LastDateModified = DateTime.Now;
                sm.PauseService(ser);
            }

            return paused;
        }

        public bool UndoDelete(int serviceid)
        {
            var sm = new ServiceManager(_db);
            var ser = sm.GetServiceByID(serviceid);
            bool success = false;

            if (ser != null)
            {
                ser.IsActive = true;
                ser.IsDeleted = false;
                ser.LastDateModified = DateTime.Now;

                success = sm.UndoDelete(ser);
            }

            return success;
        }

        public bool DeleteServiceById(int serviceid)
        {
            bool success;
            var sm = new ServiceManager(_db);
            success = sm.DeleteService(serviceid);

            return success;

        }

        public bool DeleteServicePermanently(int serviceid)
        {
            var sm = new ServiceManager(_db);
            bool success = sm.DeletePermanently(serviceid);
            return success;
        }
    }
}
