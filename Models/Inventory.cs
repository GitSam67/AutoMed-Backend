using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        public required int MedicineId { get; set; }

        public required int Quantity { get; set; }

        public required int BranchId { get; set; }
    }
}
