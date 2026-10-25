using Microsoft.EntityFrameworkCore;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;

using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Data
{
    /// <summary>
    /// Working functions
    /// </summary>
    public class ServiceManager
    {
        private static readonly HttpClient MonitoringHttpClient = CreateMonitoringHttpClient();

        private static HttpClient CreateMonitoringHttpClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VC3EYE-Monitor/1.0 (+service health check)");
            return client;
        }

        //declare database
        private readonly Vc3eyeContext _db;

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

        public Task<Service?> GetServiceByIDAsync(int serviceid, CancellationToken cancellationToken = default)
        {
            return _db.Services
                .Include(x => x.ServiceDownHistories)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ServiceId == serviceid, cancellationToken);
        }

        public List<Service> GetServiceBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));
            }

            var serviceSearch = _db.Services
                .Include(s => s.ServiceDownHistories)
                .Where(s => s.ServiceName.ToLower().Contains(searchTerm.ToLower()) && s.IsActive == true && s.IsDeleted == false)
                .ToList();

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

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }

        public async Task AddServiceDownHistoryAsync(ServiceDownHistory history, CancellationToken cancellationToken = default)
        {
            _db.ServiceDownHistories.Add(history);
            await _db.SaveChangesAsync(cancellationToken);
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
        public async Task<HttpCheckResult> CheckHttp(object? state, Service s, CancellationToken cancellationToken = default)
        {
            if (s?.Url == null || !Uri.TryCreate(s.Url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return new HttpCheckResult(false, null, "Invalid HTTP/HTTPS URL", null, 0, null);

            var timer = Stopwatch.StartNew();
            try
            {
                using var response = await MonitoringHttpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead, cancellationToken);
                timer.Stop();
                var status = (int)response.StatusCode;
                // This check measures service availability, not page correctness.
                // Sites may return access-denied pages or vary their HTML based on
                // the request source, so a configured lookup term must not mark an
                // otherwise responding service as down.
                var reachable = status < 500;
                var message = $"HTTP {status} {response.ReasonPhrase}";
                if (status is 403 or 429)
                    message += " (host reachable; request blocked or rate-limited)";
                return new HttpCheckResult(reachable, status, message, response.RequestMessage?.RequestUri?.ToString(), timer.ElapsedMilliseconds, null);
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw;
                timer.Stop();
                return new HttpCheckResult(false, null, "HTTP request timed out", uri.ToString(), timer.ElapsedMilliseconds, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                timer.Stop();
                return new HttpCheckResult(false, null, "HTTP request failed", uri.ToString(), timer.ElapsedMilliseconds, ex.Message);
            }
            catch (Exception ex)
            {
                timer.Stop();
                return new HttpCheckResult(false, null, "HTTP check error", uri.ToString(), timer.ElapsedMilliseconds, ex.Message);
            }
        }

        public async Task<HttpCheckResult> CheckConfiguredMonitorAsync(Service service, CancellationToken cancellationToken = default)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                switch ((ServiceMonitorType)service.MonitorType)
                {
                    case ServiceMonitorType.Http:
                        return await CheckHttp(service, service, cancellationToken);
                    case ServiceMonitorType.TcpPort:
                        var tcpHost = GetMonitorHost(service);
                        if (string.IsNullOrWhiteSpace(tcpHost) || service.Port is not (> 0 and <= 65535))
                            return new HttpCheckResult(false, null, "TCP monitor requires a valid host and port", null, timer.ElapsedMilliseconds, null);
                        var tcpOk = await CheckTcpEndpointAsync(tcpHost, service.Port.Value, cancellationToken);
                        return new HttpCheckResult(tcpOk, null, tcpOk ? $"TCP {tcpHost}:{service.Port} connected" : $"TCP {tcpHost}:{service.Port} connection failed", null, timer.ElapsedMilliseconds, null);
                    case ServiceMonitorType.Dns:
                        var dnsHost = GetMonitorHost(service, preferUrlHost: true);
                        if (string.IsNullOrWhiteSpace(dnsHost))
                            return new HttpCheckResult(false, null, "DNS monitor requires a hostname or IP address", null, timer.ElapsedMilliseconds, null);
                        var addresses = await Dns.GetHostAddressesAsync(dnsHost, cancellationToken);
                        var dnsMessage = addresses.Length > 0 ? $"DNS resolved {dnsHost} to {string.Join(", ", addresses.Select(a => a.ToString()).Take(4))}" : $"DNS returned no addresses for {dnsHost}";
                        return new HttpCheckResult(addresses.Length > 0, null, dnsMessage, null, timer.ElapsedMilliseconds, null);
                    case ServiceMonitorType.SslCertificate:
                        var sslHost = GetMonitorHost(service, preferUrlHost: true);
                        if (string.IsNullOrWhiteSpace(sslHost))
                            return new HttpCheckResult(false, null, "SSL monitor requires a hostname", null, timer.ElapsedMilliseconds, null);
                        return await CheckSslCertificateAsync(sslHost, service.Port ?? GetSslPort(service.Url), service.SslExpiryWarningDays, timer, cancellationToken);
                    case ServiceMonitorType.Api:
                        return await CheckApiAsync(service, timer, cancellationToken);
                    default:
                        return new HttpCheckResult(false, null, "Unknown monitor type", null, timer.ElapsedMilliseconds, null);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                timer.Stop();
                return new HttpCheckResult(false, null, "Monitor check failed", null, timer.ElapsedMilliseconds, ex.Message);
            }
        }

        private static string? GetMonitorHost(Service service, bool preferUrlHost = false)
        {
            var target = service.Url?.Trim();
            if (preferUrlHost && !string.IsNullOrWhiteSpace(target))
                return Uri.TryCreate(target, UriKind.Absolute, out var parsedUri) ? parsedUri.Host : target.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(service.Ipaddress))
                return service.Ipaddress.Trim();
            if (string.IsNullOrWhiteSpace(target)) return null;
            if (Uri.TryCreate(target, UriKind.Absolute, out var uri)) return uri.Host;
            return target.TrimEnd('/');
        }

        private static int GetSslPort(string? url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) && !uri.IsDefaultPort ? uri.Port : 443;

        private static async Task<bool> CheckTcpEndpointAsync(string host, int port, CancellationToken cancellationToken)
        {
            var addresses = IPAddress.TryParse(host, out var ip)
                ? new[] { ip }
                : await Dns.GetHostAddressesAsync(host, cancellationToken);
            foreach (var address in addresses)
            {
                try
                {
                    using var client = new TcpClient(address.AddressFamily);
                    await client.ConnectAsync(address, port, cancellationToken);
                    if (client.Connected) return true;
                }
                catch (SocketException) { }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            }
            return false;
        }

        private static async Task<HttpCheckResult> CheckSslCertificateAsync(string host, int port, int warningDays, Stopwatch timer, CancellationToken cancellationToken)
        {
            X509Certificate2? certificate = null;
            SslPolicyErrors policyErrors = SslPolicyErrors.None;
            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);
            using var ssl = new SslStream(client.GetStream(), leaveInnerStreamOpen: false, (_, cert, _, errors) =>
            {
                if (cert != null) certificate = new X509Certificate2(cert);
                policyErrors = errors;
                return true;
            });
            await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions { TargetHost = host }, cancellationToken);
            timer.Stop();
            if (certificate == null)
                return new HttpCheckResult(false, null, "TLS handshake completed without a server certificate", null, timer.ElapsedMilliseconds, null);

            using (certificate)
            {
                var expires = certificate.NotAfter;
                var daysLeft = (int)Math.Floor((expires.ToUniversalTime() - DateTime.UtcNow).TotalDays);
                if (policyErrors != SslPolicyErrors.None)
                    return new HttpCheckResult(false, null, $"Certificate validation failed: {policyErrors}", null, timer.ElapsedMilliseconds, null);
                if (daysLeft < 0)
                    return new HttpCheckResult(false, null, $"TLS certificate expired on {expires:u}", null, timer.ElapsedMilliseconds, null);
                var message = daysLeft <= warningDays
                    ? $"TLS certificate valid, expires in {daysLeft} day(s) (warning threshold: {warningDays})"
                    : $"TLS certificate valid, expires in {daysLeft} day(s)";
                return new HttpCheckResult(true, null, message, null, timer.ElapsedMilliseconds, null);
            }
        }

        private static async Task<HttpCheckResult> CheckApiAsync(Service service, Stopwatch timer, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(service.Url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return new HttpCheckResult(false, null, "API monitor requires an HTTP or HTTPS URL", null, timer.ElapsedMilliseconds, null);

            using var request = new HttpRequestMessage(new HttpMethod(service.ApiHttpMethod), uri);
            if (!string.Equals(service.ApiHttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(service.ApiRequestBody))
                request.Content = new StringContent(service.ApiRequestBody, Encoding.UTF8, "application/json");
            using var response = await MonitoringHttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var statusCode = (int)response.StatusCode;
            var expectedStatus = service.ExpectedStatusCode ?? 200;
            var bodyMatches = true;
            if (!string.IsNullOrWhiteSpace(service.ApiExpectedContent))
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                bodyMatches = body.Contains(service.ApiExpectedContent, StringComparison.OrdinalIgnoreCase);
            }
            timer.Stop();
            var healthy = statusCode == expectedStatus && bodyMatches;
            var detail = $"API {service.ApiHttpMethod} returned HTTP {statusCode} (expected {expectedStatus})";
            if (!string.IsNullOrWhiteSpace(service.ApiExpectedContent)) detail += bodyMatches ? "; expected response text found" : "; expected response text missing";
            return new HttpCheckResult(healthy, statusCode, detail, response.RequestMessage?.RequestUri?.ToString(), timer.ElapsedMilliseconds, null);
        }

        public async Task<bool> CheckHTTPStatus(object? state, Service s) => (await CheckHttp(state, s)).IsHealthy;

        public async Task<Tuple<bool, int, string>> CheckHTTPResponse(object? state, Service s)
        {
            if (s == null || s.Url == null) return Tuple.Create(false, 0, "No URL configured");
            var result = await CheckHttp(state, s);
            return Tuple.Create(result.IsHealthy, result.StatusCode ?? 0, result.Message + (result.Error == null ? "" : ": " + result.Error));
        }

        public void ApplyCheckResult(Service service, bool checkPassed, string diagnostic, int recoveryChecks = 3)
        {
            var wasRunning = service.IsRunning;
            var downThreshold = Math.Max(1, service.NumOfBackup ?? 0);
            var streak = service.RunningDownCounter;
            streak = checkPassed ? Math.Max(0, streak) + 1 : Math.Min(0, streak) - 1;
            service.RunningDownCounter = streak;
            if (!checkPassed && streak <= -downThreshold) service.IsRunning = false;
            else if (checkPassed && streak >= Math.Max(1, recoveryChecks)) service.IsRunning = true;
            if (service.IsRunning != wasRunning) service.NumOfDownTimes = 0;
            service.LastTimeChecked = DateTime.Now;
            service.NotificationMessage = diagnostic.Length <= 100 ? diagnostic : diagnostic[..100];
        }


        /// <summary>
        /// Check Service URL status through the HTTPClient and Page content - NO LOOP
        /// </summary>
        /// <param name="s">Checking Service</param>
        public async Task<bool> CheckHTTPContent(object? state, Service s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.Url) || string.IsNullOrWhiteSpace(s.LookupTerm))
                return false;

            if (!Uri.TryCreate(s.Url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return false;

            try
            {
                using var response = await MonitoringHttpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                if (!response.IsSuccessStatusCode)
                    return false;

                var html = await response.Content.ReadAsStringAsync();
                return html.Contains(s.LookupTerm, StringComparison.OrdinalIgnoreCase);
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
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
        public async Task<bool> CheckPort(object? state, Service s, CancellationToken cancellationToken = default)
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
                    ipAddress = await Dns.GetHostEntryAsync(s.Ipaddress, cancellationToken).ConfigureAwait(false);
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
                        IPAddress[] addresses = await Dns.GetHostAddressesAsync(uri.Host, cancellationToken);
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
                    await socket.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);
                    return socket.Connected;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
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
            // ICMP is informational: DNS and ping failures are ordinary negative
            // results and should not be recorded as application errors.
            try
            {
                IPAddress? address = null;

                if (!string.IsNullOrWhiteSpace(s.Ipaddress) && IPAddress.TryParse(s.Ipaddress, out var configuredAddress))
                {
                    address = configuredAddress;
                }
                else if (!string.IsNullOrWhiteSpace(s.Url) && Uri.TryCreate(s.Url, UriKind.Absolute, out var uri) &&
                         (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    // Prefer IPv4 where available; some targets publish IPv6
                    // addresses the monitoring host cannot route or ping.
                    var addresses = Dns.GetHostAddresses(uri.Host);
                    address = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                        ?? addresses.FirstOrDefault();
                }

                if (address == null)
                {
                    s.IsIcmpRunning = false;
                    return false;
                }

                using var ping = new Ping();
                var reply = ping.Send(address, 3000);
                s.IsIcmpRunning = reply.Status == IPStatus.Success;
            }
            catch (SocketException)
            {
                // DNS resolution or network lookup failed; ICMP remains informational.
                s.IsIcmpRunning = false;
            }
            catch (PingException)
            {
                s.IsIcmpRunning = false;
            }
            catch (Exception ex)
            {
                s.IsIcmpRunning = false;
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

    public sealed record HttpCheckResult(bool IsHealthy, int? StatusCode, string Message, string? FinalUrl, long ElapsedMilliseconds, string? Error);
}
