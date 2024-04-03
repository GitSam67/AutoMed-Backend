using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMed_Backend.EventHandler
{

    public delegate void MedicineEventHandler(object sender, MedicineEventArgs e);

    // Define a class to hold event data
    public class MedicineEventArgs : EventArgs
    {
        public string MedicineName { get; set; }

        public MedicineEventArgs(string medicineName)
        {
            MedicineName = medicineName;
        }
    }
}
