using AutoMed_Backend.Models;
using AutoMed_Backend.EventHandler;

namespace AutoMed_Backend.Repositories
{
    public class EventLogic
    {
        StoreDbContext ctx;

        public EventLogic(StoreDbContext ctx) 
        { 
            this.ctx = ctx;
        }
        public void CheckEventAlert()
        {
            MedicineInventory inventoryhandler = new MedicineInventory();
            MedicineEventListener listener = new MedicineEventListener(inventoryhandler);

            var _medicines = ctx.Medicines.ToList();
            var _inventory = ctx.Inventory.ToList();
            var _MedResult = from m in _medicines select m;

            Console.WriteLine($"\n{new string(' ', 20)}** Alert **");
            Console.WriteLine($"{new string('-', 55)}");
            foreach (var res in _MedResult)
            {
                var _stock = (from i in _inventory where res.MedicineId == i.MedicineId select i.Quantity).FirstOrDefault();
                inventoryhandler.CheckMedicineStatus(res.Name, res.ExpiryDate, _stock);
            }
            Console.WriteLine($"\n{new string('-', 55)}");
        }
    }
}
