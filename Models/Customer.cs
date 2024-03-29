using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string? CustomerName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public string? BloodGroup { get; set; }
        [Required]
        public string? ContactNo { get; set; }

        public string? Prescription { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
