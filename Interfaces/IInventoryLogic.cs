using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface IInventoryLogic
    {
        public Task<SingleObjectResponse<Medicine>> PlaceOrder(Dictionary<string, int> orders, int branchId);
        public Task<Dictionary<string, int>> GetInventoryDetails(int branchId);
        public decimal GetCashBalance(int branchId);
        public decimal GetTotalSales(int branchId);
    }
}
