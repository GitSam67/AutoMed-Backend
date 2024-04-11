using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [NumericNonNegative(ErrorMessage = "Age must be numeric and non negative")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "BloodGroup is required")]
        public string? BloodGroup { get; set; }

        [Required(ErrorMessage = "ContactNo is required")]
        [NumericNonNegative(ErrorMessage = "Contact Number must be numeric and non negative")]
        public string? ContactNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Prescription is required")]
        public string? Prescription { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailFormat(ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

    }
}
