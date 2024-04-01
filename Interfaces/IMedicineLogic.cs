using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface IMedicineLogic
    {
        public Task<CollectionResponse<Medicine>> GetMedicinesList();
        public Task<SingleObjectResponse<Medicine>> AddMedicine(Medicine med);
        public Task<SingleObjectResponse<Medicine>> UpdateMedicine(int id, Medicine med);
        public Task<SingleObjectResponse<Medicine>> RemoveMedicine(int id);
    }
}
