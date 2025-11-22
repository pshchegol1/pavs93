using System.ComponentModel.DataAnnotations;

namespace LoginDashboardApp.Models
{
    public class SettingsModel
    {
        [Required]
        public int SettingId { get; set; }

        [Required]
        public int? SmtpPort { get; set; }

        [Required]
        public string? SmtpUserName { get; set; }

        [Required]
        public string? SmtpPassword { get; set; }

        [Required]
        public string? MtClientId { get; set; }

        [Required]
        public string? MtSecretKey { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        [Required]
        public virtual ICollection<User> Users { get; } = new List<User>();
    }
}
