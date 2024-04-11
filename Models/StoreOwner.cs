using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class StoreOwner
    {
        [Key]
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Owner name is required")]
        public string? OwnerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Branch id is required")]
        [NumericNonNegative(ErrorMessage = "Branch id must be numeric and non negative")]
        public int BranchId { get; set; }

    }
}
