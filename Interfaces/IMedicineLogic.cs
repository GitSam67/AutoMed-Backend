using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface IMedicineLogic
    {
        public void AddMedicine(Medicine med);
        public Task<Medicine> UpdateMedicine(int id, Medicine med);
        public void RemoveMedicine(int id);
    }
}
