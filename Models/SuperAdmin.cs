using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class SuperAdmin
    {
        [Key]
        public int AdminId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
