using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]

        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

        public DateTime? PurchaseTime { get; set; }

        public decimal? TotalBill { get; set; }
        
        [Required]
        public int BranchId { get; set; }
    }
}
