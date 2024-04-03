using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMed_Backend.EventHandler
{
    public class MedicineEventListener
    {
        // Constructor to subscribe to the event
        public MedicineEventListener(MedicineInventory inventory)
        {
            // Subscribe to the event
            inventory.MedicineExpiring += Inventory_MedicineExpiring;
            inventory.MedicineOutOfStock += Inventory_MedicineOutOfStock;
        }

        // Event handler for medicine expired or out-of-stock event
        private void Inventory_MedicineExpiring(object sender, MedicineEventArgs e)
        {
            Console.WriteLine($"\n* Medicine '{e.MedicineName}' is expiring soon...");
            Console.WriteLine("  Please discard them from inventory asap.");
        }
        private void Inventory_MedicineOutOfStock(object sender, MedicineEventArgs e)
        {
            Console.WriteLine($"\n* Medicine '{e.MedicineName}' is out of stock...");
            Console.WriteLine(" Please restock them in store inventory.");
        }
    }
}
