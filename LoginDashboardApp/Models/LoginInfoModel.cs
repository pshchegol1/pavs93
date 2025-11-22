using System.ComponentModel.DataAnnotations;

namespace LoginDashboardApp.Models
{
    public class LoginInfoModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? LoginError { get; set; }
    }
}
