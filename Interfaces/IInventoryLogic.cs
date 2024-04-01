using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface IInventoryLogic
    {
        public void PlaceOrder(Dictionary<Medicine, int> orders);
        public Task<Dictionary<Medicine, int>> GetInventoryDetails();
        public decimal GetCashBalance();
        public decimal GetTotalSales();
    }
}
