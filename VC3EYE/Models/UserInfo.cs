using System.ComponentModel.DataAnnotations;
using VC3EYE.Entities;

namespace VC3EYE.Models
{
    public class UserInfo
    {

        public int UserId { get; set; }
        [Required(ErrorMessage = "First Name is required")]

        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last Name is required")]

        public string LastName { get; set; } = null!;


        public string City { get; set; } = null!;

        public string ProvinceState { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]

        public string? Email { get; set; }
        [Required(ErrorMessage = "RoleId  is required")]

        public int RoleId { get; set; }
        [Required(ErrorMessage = " Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "UserName is required")]

        public string? UserName { get; set; }

        public int? SettingId { get; set; }

        public virtual Log? Log { get; set; }
        [Required(ErrorMessage = "Role is required")]

        public virtual Role Role { get; set; } = null!;


        public virtual ICollection<Service> Services { get; } = new List<Service>();

        public virtual Setting? Setting { get; set; }

        
    }
}
