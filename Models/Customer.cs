using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        
        public string? CustomerName { get; set; }
        
        public int Age { get; set; }
        
        public string? Gender { get; set; }
       
        public string? BloodGroup { get; set; }
        
        public string? ContactNo { get; set; }
        
        public string? Address { get; set; }

        public string? Prescription { get; set; }
        
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
