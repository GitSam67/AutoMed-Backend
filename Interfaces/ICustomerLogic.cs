using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ICustomerLogic
    {
        public Task<SingleObjectResponse<Customer>> AddCustomer(Customer c);
        public Task<decimal> GenerateMedicalBill(Customer c, Dictionary<string, int> orders, decimal claim);
        public void ExecuteOrder(Customer c, Dictionary<string, int> orders, decimal bill);
    }
}
