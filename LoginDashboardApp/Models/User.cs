using System.Data;

namespace LoginDashboardApp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string City { get; set; } = null!;

        public string ProvinceState { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int RoleId { get; set; }

        public string Password { get; set; } = null!;

        public string? UserName { get; set; }

        public int? SettingId { get; set; }

        public virtual ICollection<Log> Logs { get; } = new List<Log>();

        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<Service> Services { get; } = new List<Service>();

        public virtual Setting? Setting { get; set; }
    }
}
