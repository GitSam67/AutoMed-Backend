using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
        public List<string> orders { get; set; } = new List<string>();

        public DateTime? PurchaseTime { get; set; }

        public decimal? TotalBill { get; set; }
        
        public int BranchId { get; set; }
    }
}
