using AutoMed_Backend.Models;

namespace AutoMed_Backend.Repositories
{
    public interface IAdminRepository<TEntity, in TPk> where TEntity : EntityBase
    {
        void AddMedicine(Medicine med);
        Task<SingleObjectResponse<TEntity>> UpdateMedicine(TPk id, Medicine med);
        void RemoveMedicine(TPk id);
        void PlaceOrder(string name, int qty);
        void GetInventoryDetails();
        decimal GetCashBalance();
        decimal GetTotalSales();
        void CreateSalesReport(TEntity entity, Dictionary<string, int> orders, decimal bill);
        void GeneratePaymentRecord(TEntity entity, Dictionary<string, int> orders, decimal bill, string mode);
     
    }
}
