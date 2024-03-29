using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
