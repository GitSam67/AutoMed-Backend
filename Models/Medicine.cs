using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        public required string? Name { get; set; }

        public required decimal UnitPrice { get; set; }

        public required DateTime ExpiryDate { get; set; }

        public required string? BatchNumber { get; set; }

        public required string? Manufacturer { get; set; }

        public required string? Category { get; set; }

    }
}
