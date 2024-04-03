using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMed_Backend.EventHandler
{
    public class MedicineInventory
    {
        // Declare the event using our delegate
        public event MedicineEventHandler? MedicineExpiring;
        public event MedicineEventHandler? MedicineOutOfStock;

        // Method to check medicine expiration and stock status
        public void CheckMedicineStatus(string medicineName, DateTime expirationDate, int stock)
        {
            if (expirationDate <= DateTime.Now.AddDays(10))
            {
                OnMedicineExpiring(new MedicineEventArgs(medicineName));
            }
            else if (stock <= 0)
            {
                OnMedicineOutOfStock(new MedicineEventArgs(medicineName));
            }
        }

        // Method to raise the event
        protected virtual void OnMedicineExpiring(MedicineEventArgs e)
        {
            MedicineExpiring?.Invoke(this, e);
        }
        protected virtual void OnMedicineOutOfStock(MedicineEventArgs e)
        {
            MedicineOutOfStock?.Invoke(this, e);
        }
    }
}
