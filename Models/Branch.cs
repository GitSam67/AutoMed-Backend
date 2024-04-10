using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        
        public required string? BranchName { get; set; }

        public required string? Address { get; set; }
    }
}
