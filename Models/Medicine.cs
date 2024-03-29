using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public string? BatchNumber { get; set; }

        [Required]
        public string? Manufacturer { get; set; }

        [Required]
        public string? Category { get; set; }



    }
}
