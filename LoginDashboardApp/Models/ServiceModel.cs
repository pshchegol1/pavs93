using System.ComponentModel.DataAnnotations;

namespace LoginDashboardApp.Models
{
    public class ServiceModel
    {
        public int ServiceID { get; set; }

        [Required(ErrorMessage = "Service Name is required")]
        public string? ServiceName { get; set; }

        //[Required(ErrorMessage = "URL is required")]
        [DataType(DataType.Url)]
        public string? ServiceURL { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string? Location { get; set; }

        //check IP address in a separate function later
        public string? IPAddress { get; set; }

        public string? GeneralInformation { get; set; }

        public Boolean IsImportant { get; set; }

        public Boolean IsActive { get; set; }

        [RegularExpression("^([1-9]|[1-5]\\d{0,3}|6[0-4]\\d{0,2}|653[0-4]\\d?|6535[0-3])$", ErrorMessage = "Please enter valid Port")]
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
    }
}
