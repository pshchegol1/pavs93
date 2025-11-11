using Microsoft.EntityFrameworkCore;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using VC3EYE.Entities;

namespace VC3EYE.Data
{
    /// <summary>
    /// Working functions
    /// </summary>
    public class ServiceManager
    {
        //declare database
        private readonly Vc3eyeContext _db;

        //lock
        private Object thisLock = new Object();

        public ServiceManager(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = _db.Users.Include(x => x.Role).ToList();

            return users;
        }

        public List<Service> GetAllServices()
        {
            List<Service> services = _db.Services.Include(x => x.User).Include(x => x.ServiceDownHistories).Where(x => x.IsActive == true && x.IsDeleted == false).ToList();

            return services;
        }

        public Service? GetServiceByID(int serviceid)
        {
            var service = _db.Services.Include(x => x.ServiceDownHistories).Include(x => x.User).Where(x => x.ServiceId == serviceid).FirstOrDefault();

            return service;
        }

        public List<Service> GetServiceBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));
            }

            var serviceSearch = _db.Services.Where(s => s.ServiceName.ToLower().Contains(searchTerm.ToLower()) && s.IsActive == true && s.IsDeleted == false).ToList();

            return serviceSearch;
        }

        public List<string> GetAllLocations()
        {
            List<string> loc = _db.Services.Select(x => x.Location).Distinct().ToList();

            return loc;
        }

        public List<Service> GetDeletedServices()
        {
            List<Service> deletedServices = _db.Services.Include(x => x.User).Where(x => x.IsActive == false && x.IsDeleted == true).ToList();
            return deletedServices;
        }

        public List<Service> GetAllPausedServices()
        {
            List<Service> pausedServices = _db.Services.Include(x => x.User).Where(x => x.IsActive == false && x.IsDeleted == false).ToList();
            return pausedServices;
        }

        public List<Service> GetServiceByLocation(string locationVal)
        {
            var location = _db.Services.Include(x => x.ServiceDownHistories).Where(l => l.Location.ToLower().Contains(locationVal.ToLower()) && l.IsActive == true && l.IsDeleted == false).ToList();

            return location;
        }

        public bool DeleteService(int serviceid)
        {
            bool Success = false;

            var service = _db.Services.Include(x => x.User)
                .Where(x => x.ServiceId == serviceid && x.IsActive == true && x.IsDeleted == false)
                .FirstOrDefault();

            if (service != null)
            {
                service.IsActive = false;
                service.IsDeleted = true;

                if (_db.SaveChanges() >= 0)
                {
                    Success = true;
                }
            }

            return Success;
        }

        public bool UpdateServiceStatus(Service service)
        {
            bool updated = false;

            if (_db.SaveChanges() >= 0)
            {
                updated = true;
            }

            return updated;
        }

        public bool PauseService(Service service)
        {
            bool paused = false;

            if (_db.SaveChanges() >= 0)
            {
                paused = true;
            }

            return paused;
        }

        public bool UndoDelete(Service service)
        {
            bool success = false;

            if (_db.SaveChanges() >= 0)
            {
                success = true;
            }
            return success;
        }

        public bool AddService(Service service)
        {
            //flag
            bool success = false;

            try
            {
                _db.Services.Add(service);

                if (_db.SaveChanges() >= 0)
                {
                    success = true;
                }
            }
            catch (Exception)
            {

            }

            return success;
        }

        public bool ModifyService(Service service)
        {
            bool updated = false;

            try
            {
                if (service != null)
                {
                    if (_db.SaveChanges() >= 0)
                    {
                        updated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to Modify Services in Services Thread",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return updated;
        }

        /// <summary>
        /// Check RSS Feeds of the Service
        /// </summary>
        public bool CheckRSS(string serviceRssURL)
        {
            //try catch
            try
            {
                // Create an XmlDocument object and load the RSS feed from the URL
                XmlDocument rssXMLDoc = new();

                // Might error when loading
                rssXMLDoc.Load(serviceRssURL);

                if (rssXMLDoc.DocumentElement != null)
                {
                    // Select all of the items in the RSS feed
                    XmlNodeList? rssItems = rssXMLDoc.SelectNodes("rss/channel/item");

                    // Create a StringBuilder to store the output
                    StringBuilder output = new StringBuilder();

                    if (rssItems == null)
                    {
                        // If there are no items, return false
                        return false;
                    }
                    // Iterate through each item in the RSS feed
                    foreach (XmlNode rssItem in rssItems)
                    {
                        // Extract the title of the RSS item
                        XmlNode? titleNode = rssItem.SelectSingleNode("title");
                        if (titleNode == null)
                        {
                            // If the title is null, skip this item
                            continue;
                        }
                        string title = titleNode.InnerText;

                        // Extract the link of the RSS item
                        XmlNode? linkNode = rssItem.SelectSingleNode("link");
                        if (linkNode == null)
                        {
                            // If the link is null, skip this item
                            continue;
                        }
                        string link = linkNode.InnerText;

                        // Extract the description of the RSS item
                        XmlNode? descriptionNode = rssItem.SelectSingleNode("description");
                        if (descriptionNode == null)
                        {
                            // If the description is null, skip this item
                            continue;
                        }
                        string description = descriptionNode.InnerText;

                        // Extract the publication date of the RSS item
                        XmlNode? pubDateNode = rssItem.SelectSingleNode("pubDate");
                        if (pubDateNode == null)
                        {
                            // If the publication date is null, skip this item
                            continue;
                        }
                        string pubDate = pubDateNode.InnerText;

                        // Append the title, link, description, and publication date to the output
                        output.AppendLine("Title: " + title);
                        output.AppendLine("Link: " + link);
                        output.AppendLine("Description: " + description);
                        output.AppendLine("Publication Date: " + pubDate);
                        output.AppendLine();
                    }

                    // Output the results
                    Console.WriteLine(output.ToString());

                    // Wait for the user to press a key before closing the console window
                    Console.ReadLine();

                    // Return true if the RSS feed was loaded and parsed successfully
                    return true;
                }
            }

            catch (Exception ex)
            {
                // If an error occurs while loading the RSS feed, print an error message and exit
                Console.WriteLine("Failed to load RSS feed: " + ex.Message);

                // Return false if an error occurred
                return false;
            }

            // Return false if no RSS feed was loaded
            return false;
        }

        /// <summary>
        /// Check Service URL status through the HTTPClient and Page content - NO LOOP
        /// </summary>
        /// <param name="s">Checking Service</param>
        public async Task<bool> CheckHTTPStatus(object? state, Service s)
        {
            //get meta info
            if (s == null || s.Url == null) { return false; }

            string url = s.Url;

            // Check if the URL is valid
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Create a new instance of HttpClient, which will be reused for all requests
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Send an HTTP GET request to the service website
                        HttpResponseMessage response = await client.GetAsync(url);

                        //Conver URL string to URI
                        Uri siteUri = new Uri(url);

                        // Ensure that the response status code is in the 2xx range, indicating success
                        response.EnsureSuccessStatusCode();
                    }
                    catch
                    {
                        //return flag for this check
                        return false;
                    }
                }

                //happy
                return true;
            }

            //end flag of the URI check
            else return false;
        }

        public async Task<Tuple<bool, int, string>> CheckHTTPResponse(object? state, Service s)
        {
            //initial
            Tuple<bool, int, string> obj = new Tuple<bool, int, string>(false, 123, "No Feedback");

            //get meta info
            if (s == null || s.Url == null) { return obj; }

            string url = s.Url;

            // Check if the URL is valid
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Create a new instance of HttpClient, which will be reused for all requests
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Send an HTTP GET request to the service website
                        HttpResponseMessage response = await client.GetAsync(url);

                        //Conver URL string to URI
                        Uri siteUri = new Uri(url);

                        //get the code
                        int code = (int)response.StatusCode;

                        // Ensure that the response status code is in the 2xx range, indicating success
                        bool success = response.IsSuccessStatusCode;

                        //response.EnsureSuccessStatusCode();

                        obj = new Tuple<bool, int, string>(success, code, response.ReasonPhrase ?? string.Empty);

                        return obj;
                    }
                    catch
                    {
                        //return flag for this check
                        return obj;
                    }
                }

            }

            //end flag of the URI check
            else return obj;
        }


        /// <summary>
        /// Check Service URL status through the HTTPClient and Page content - NO LOOP
        /// </summary>
        /// <param name="s">Checking Service</param>
        public async Task<bool> CheckHTTPContent(object? state, Service s)
        {
            //get meta info
            if (s == null || s.Url == null || s.LookupTerm == null) { return false; }
            string url = s.Url;
            string stringLookup = s.LookupTerm;

            bool success = false;

            // Check if the URL is valid
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Create a new instance of HttpClient, which will be reused for all requests
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Create an HttpClient instance to download the HTML content of the website
                        HttpClient clientHttp = new HttpClient();
                        string htmlContent = await client.GetStringAsync(url);

                        //Console.WriteLine(htmlContent);

                        success = htmlContent.IndexOf(s.LookupTerm, StringComparison.OrdinalIgnoreCase) >= 0;

                        return success;
                    }
                    catch
                    {
                        //return flag for this check
                        return false;
                    }
                }
            }

            //end flag of the URI check
            else return success;
        }

        /// <summary>
        /// PING check service
        /// </summary>
        /// <param name="state"></param>
        /// <param name="s">Checking service</param>
        /// <returns></returns>
        public async Task<bool> CheckPing(object? state, Service s)
        {
            //null check
            if (s == null || s.Ipaddress == null) return false;

            //parse IP
            IPAddress? iP = null;

            //catch TryParse exception
            try
            {
                IPAddress.TryParse(s.Ipaddress, out iP);
            }
            catch
            {
                return false;
            }

            //null check
            if (iP == null) return false;

            //catching net logic, error means failed
            try
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(iP);
                if (reply.Status == IPStatus.Success) return true;
                else return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check specific port if it is open
        /// </summary>
        /// <param name="state"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<bool> CheckPort(object? state, Service s)
        {
            //null check
            if (s == null) return false;

            //catching net logic, error means failed
            try
            {
                //check
                if (s.Port == null) return false;

                int portNumber = (int)s.Port;

                IPHostEntry? ipAddress = null;
                IPEndPoint? endPoint = null;
                IPAddress? ipCheck = null;

                //valid port is checked when input
                //resolve ip from either ip or URL
                if (!string.IsNullOrEmpty(s.Ipaddress))
                {
                    ipAddress = await Dns.GetHostEntryAsync(s.Ipaddress).ConfigureAwait(false);
                    ipCheck = ipAddress.AddressList[0];
                }

                //check if url is https or http
                else if (!string.IsNullOrEmpty(s.Url))
                {
                    //uri check
                    Uri? uri = null;
                    //if uri is valid, process convert
                    if (Uri.TryCreate(s.Url, UriKind.Absolute, out uri))
                    {
                        IPAddress[] addresses = await Dns.GetHostAddressesAsync(uri.Host);
                        if (addresses.Length > 0)
                        {
                            ipCheck = addresses[0];
                        }
                        else return false;
                    }

                    //URI failed
                    else return false;
                }

                //if no ip and url
                else
                    return false;

                endPoint = new IPEndPoint(ipCheck, portNumber);

                using (var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    await socket.ConnectAsync(endPoint).ConfigureAwait(false);
                    return socket.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MSTeamsNotify(Setting st, Service s)
        {
            //string TeamsID = "https://naitca.webhook.office.com/webhookb2/b1c5582c-67d6-4734-871f-ee106d1ab12d@5c98fb47-d3b9-4649-9d94-f88cbdd9729c/IncomingWebhook/acd93de71184419eae35286612a89a27/f85f89e7-cb4a-485d-87dc-e6c1627b5cea";

            //null check
            if (st.MtSecretKey == null || s == null) return false;

            string teamsID = st.MtSecretKey;

            //convert status string
            string sURL = "N/A";
            string sIP = "N/A";
            string sRun = "Up";

            string sReason = "N/A";

            //logic
            if (!s.IsRunning) { sRun = "Down"; }
            if (!string.IsNullOrEmpty(s.Url)) { sURL = s.Url; }
            if (!string.IsNullOrEmpty(s.Ipaddress)) { sIP = s.Ipaddress; }
            if (!string.IsNullOrEmpty(s.NotificationMessage)) { sReason = s.NotificationMessage; }

            //core string
            string sFormatedJSON = @"{
                ""@type"": ""MessageCard"",       
                ""@context"": ""http://schema.org/extensions"",
                ""themeColor"": ""0076D7"",
                ""summary"": ""Response Message"",
                ""sections"": 
                    [{
                        ""activityTitle"": ""Service " + s.ServiceName + @""",
                        ""activitySubtitle"": ""VC3 EYE Project"",
                        ""facts"": [{
                            ""name"": ""User Added"",
                            ""value"": "" " + s.User.LastName + ", " + s.User.FirstName + @"""
                            }, {
                            ""name"": ""Last Time Checked"",
                            ""value"": "" " + s.LastTimeChecked + @"""
                            }, {
                            ""name"": ""Status"",
                            ""value"": "" " + sRun + @"""
                            }, {
                            ""name"": ""Service URL"",
                            ""value"": "" " + sURL + @"""
                            }, {
                            ""name"": ""Service IP"",
                            ""value"": "" " + sIP + @"""
                            }, {
                            ""name"": ""Service Check"",
                            ""value"": "" " + sReason + @"""
                        }]
                    }]
                }";


            await SendJSONToURL(teamsID, sFormatedJSON);

            //return
            return true;
        }

        public async Task SendJSONToURL(string url, string json)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
            }
        }

        public bool DeletePermanently(int serviceid)
        {
            bool deleted = false;

            try
            {
                var service = _db.Services.Where(x => x.ServiceId == serviceid).FirstOrDefault();

                if (service != null)
                {
                    _db.Services.Remove(service);
                    if (_db.SaveChanges() >= 0)
                    {
                        deleted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to delete service in DeletePermanently()",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return deleted;
        }

        public bool CheckICMP(Service s)
        {
            //run
            try
            {
                if (!string.IsNullOrEmpty(s.Url))
                {
                    // uri check
                    Uri? uri;
                    if (Uri.TryCreate(s.Url, UriKind.Absolute, out uri))
                    {
                        IPAddress[] addresses = Dns.GetHostAddresses(uri.Host);
                        if (addresses.Length > 0)
                        {
                            // use the first IP address returned by DNS
                            var ipAddress = addresses[0];

                            // ICMP check
                            using (var ping = new Ping())
                            {
                                // validate IP address
                                if (IPAddress.TryParse(ipAddress.ToString(), out IPAddress validIpAddress))
                                {
                                    var reply = ping.Send(validIpAddress);
                                    s.IsIcmpRunning = reply.Status == IPStatus.Success;
                                }
                                else
                                {
                                    s.IsIcmpRunning = false;
                                }
                            }
                        }
                        else
                        {
                            s.IsIcmpRunning = false;
                        }
                    }
                }

                else if (!string.IsNullOrEmpty(s.Ipaddress) && Regex.IsMatch(s.Ipaddress, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"))
                {
                    // IP address check
                    if (IPAddress.TryParse(s.Ipaddress, out IPAddress validIpAddress))
                    {
                        // ICMP check
                        using (var ping = new Ping())
                        {
                            var reply = ping.Send(validIpAddress);
                            s.IsIcmpRunning = reply.Status == IPStatus.Success;
                        }
                    }
                    else
                    {
                        s.IsIcmpRunning = false;
                    }
                }
                else
                {
                    s.IsIcmpRunning = false;
                }
            }

            catch (Exception ex)
            {
                var lm = new LogsManager(_db);
                var errLog = new ErrorLog()
                {
                    ErrorName = "Failed to ICMP Check in Services Thread",
                    ErrorDescription = ex.Message,
                    DateAdded = DateTime.Now
                };

                lm.AddErrorLog(errLog);
            }

            return s.IsIcmpRunning;
        }
    }
}
