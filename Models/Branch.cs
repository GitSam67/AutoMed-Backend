using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        
        public string? BranchName { get; set; }

        public string? Address { get; set; }
    }
}
