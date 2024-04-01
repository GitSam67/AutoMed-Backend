using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class StoreOwner
    {
        [Key]
        public int OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int BranchId { get; set; }

    }
}
