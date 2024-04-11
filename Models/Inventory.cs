using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Medicine id is required")]
        [NumericNonNegative(ErrorMessage = "Medicine id must be numeric and non negative")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [NumericNonNegative(ErrorMessage = "Quantity must be numeric and non negative")]
        public required int Quantity { get; set; }

        [Required(ErrorMessage = "Branch id is required")]
        [NumericNonNegative(ErrorMessage = "Branch id must be numeric and non negative")]
        public required int BranchId { get; set; }
    }
}
