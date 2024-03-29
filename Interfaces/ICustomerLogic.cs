using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ICustomerLogic
    {
        public void AddCustomer(Customer c);
        public Task<decimal> GenerateMedicalBill(Customer c, Dictionary<string, int> orders, decimal claim);
        public void ExecuteOrder(Dictionary<string, int> orders, decimal bill);
    }
}
