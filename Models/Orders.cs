using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Customer id is required")]
        [NumericNonNegative(ErrorMessage = "Customer id must be numeric and non negative")]
        public int CustomerId { get; set; }

        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

        public List<string> orders { get; set; } = new List<string>();

        [Required(ErrorMessage = "Purchase time is required")]
        public DateTime? PurchaseTime { get; set; }

        [Required(ErrorMessage = "Total bill is required")]
        [NumericNonNegative(ErrorMessage = "Total bill must be numeric and non negative")]
        public decimal? TotalBill { get; set; }

        [Required(ErrorMessage = "Branch id is required")]
        [NumericNonNegative(ErrorMessage = "Branch id must be numeric and non negative")]
        public int BranchId { get; set; }
    }
}
