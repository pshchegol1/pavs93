using System.ComponentModel.DataAnnotations;

namespace VC3EYE.Models
{
    using System.Text.Json;

    public class ServiceModel : IValidatableObject
    {
        public int ServiceID { get; set; }

        [Required(ErrorMessage = "Service Name is required")]
        public string? ServiceName { get; set; }

        //[Required(ErrorMessage = "URL is required")]
        public string? ServiceURL { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string? Location { get; set; }

        //check IP address in a separate function later
        public string? IPAddress { get; set; }

        public string? GeneralInformation { get; set; }

        public Boolean IsImportant { get; set; }

        public Boolean IsActive { get; set; }

        [Range(1, 65535, ErrorMessage = "Enter a port between 1 and 65535.")]
        public int? Port { get; set; }

        public string? RSSFeedLink { get; set; }

        [Required(ErrorMessage = "Check Interval is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        public string? CheckInterval { get; set; }

        public int? NumOfDownTimes { get; set; }

        public int? NumOfBackup { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime LastDateModified { get; set; }

        public DateTime LastTimeChecked { get; set; }

        public string? Status { get; set; }
        public bool IsInternal { get; set; }

        public bool IsChecked { get; set; } = false;

        public int UserID { get; set; }

        public bool IsRunning { get; set; } = false;

        public string? NotificationMessage { get; set; }

        public bool IsDeleted { get; set; }

        public bool NotifyByMSTeams { get; set; }

        public string? LookupTerm { get; set; }

        public ServiceMonitorType MonitorType { get; set; } = ServiceMonitorType.Http;

        [Range(100, 599, ErrorMessage = "Enter a valid HTTP status code (100–599).")]
        public int? ExpectedStatusCode { get; set; } = 200;

        [RegularExpression("^(GET|POST|PUT|PATCH|DELETE)$", ErrorMessage = "Choose a supported API method.")]
        public string ApiHttpMethod { get; set; } = "GET";

        [StringLength(200, ErrorMessage = "Expected response text must be 200 characters or fewer.")]
        public string? ApiExpectedContent { get; set; }

        [StringLength(1000, ErrorMessage = "Request body must be 1000 characters or fewer.")]
        public string? ApiRequestBody { get; set; }

        [Range(1, 365, ErrorMessage = "Certificate warning must be between 1 and 365 days.")]
        public int SslExpiryWarningDays { get; set; } = 14;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasUrl = !string.IsNullOrWhiteSpace(ServiceURL);
            var hasIp = !string.IsNullOrWhiteSpace(IPAddress);
            if (!hasUrl && !hasIp)
                yield return new ValidationResult("Enter a target URL, hostname, or IP address.", new[] { nameof(ServiceURL), nameof(IPAddress) });

            if (MonitorType is ServiceMonitorType.Http or ServiceMonitorType.Api)
            {
                if (!Uri.TryCreate(ServiceURL, UriKind.Absolute, out var targetUri) ||
                    (targetUri.Scheme != Uri.UriSchemeHttp && targetUri.Scheme != Uri.UriSchemeHttps))
                    yield return new ValidationResult("Enter a complete HTTP or HTTPS URL for this monitor type.", new[] { nameof(ServiceURL) });
            }

            if (MonitorType == ServiceMonitorType.TcpPort && Port is not (> 0 and <= 65535))
                yield return new ValidationResult("Enter a TCP port between 1 and 65535.", new[] { nameof(Port) });

            if (MonitorType == ServiceMonitorType.Api && ExpectedStatusCode == null)
                yield return new ValidationResult("Enter the expected API status code.", new[] { nameof(ExpectedStatusCode) });

            if (!Enum.IsDefined(typeof(ServiceMonitorType), MonitorType))
                yield return new ValidationResult("Choose a supported monitor type.", new[] { nameof(MonitorType) });

            if (MonitorType == ServiceMonitorType.Api && !string.Equals(ApiHttpMethod, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(ApiRequestBody))
            {
                var validJson = true;
                try { using var _ = JsonDocument.Parse(ApiRequestBody); }
                catch (JsonException) { validJson = false; }
                if (!validJson)
                    yield return new ValidationResult("Enter a valid JSON request body.", new[] { nameof(ApiRequestBody) });
            }
        }
    }
}
