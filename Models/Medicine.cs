using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [NumericNonNegative(ErrorMessage = "Unit price must be numeric and non negative")]
        public decimal UnitPrice { get; set; }

        [ExpiryDateValidation(ErrorMessage = "Invalid expiry date format")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Batch number is required")]
        [NumericNonNegative(ErrorMessage = "Batch number must be numeric and non negative")]
        public string? BatchNumber { get; set; }

        [Required(ErrorMessage = "Manufacturer is required")]
        public string? Manufacturer { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string? Category { get; set; }

    }
}
