using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Branch name is required")]
        public string? BranchName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
    }
}
