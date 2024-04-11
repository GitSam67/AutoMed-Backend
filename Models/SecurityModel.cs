using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class AppUser
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailFormat(ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [PasswordValidation(ErrorMessage = "Password must be 8-20 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        public string? ConfirmPassword { get; set; }
    }

    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailFormat(ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
    public class RoleInfo
    {
        [Required(ErrorMessage = "Role info is required")]
        public string? Name { get; set; }
    }

    public class UserRole
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailFormat(ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string? RoleName { get; set; }
    }
}
