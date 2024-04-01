using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string? BatchNumber { get; set; }

        public string? Manufacturer { get; set; }

        public string? Category { get; set; }



    }
}
