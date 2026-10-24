using System.ComponentModel.DataAnnotations;

namespace VC3EYE.Models
{
    public class SettingsModel
    {
        public int SettingId { get; set; }

        [Range(1, 65535, ErrorMessage = "Enter a valid SMTP port from 1 to 65535.")]
        public int? SmtpPort { get; set; }

        [StringLength(100)]
        public string? SmtpUserName { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100)]
        public string? SmtpPassword { get; set; }

        [StringLength(100)]
        public string? MtClientId { get; set; }

        [DataType(DataType.Password)]
        [StringLength(500)]
        public string? MtSecretKey { get; set; }

        public bool ClearSmtpPassword { get; set; }
        public bool ClearTeamsSecret { get; set; }
    }
}
