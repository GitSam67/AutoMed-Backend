using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        public int MedicineId { get; set; }

        public int Quantity { get; set; }

        public int BranchId { get; set; }
    }
}
