using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class AppUser
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Role { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? ConfirmPassword { get; set; }
    }

    public class LoginUser
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
    public class RoleInfo
    {
        public string? Name { get; set; }
    }

    public class UserRole
    {
        public string? Email { get; set; }
        public string? RoleName { get; set; }
    }
}
