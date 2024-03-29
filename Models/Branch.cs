using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        
        [Required]
        public string? BranchName { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public int InventoryId { get; set; }
    }
}
