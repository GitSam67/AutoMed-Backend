using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ICustomerLogic
    {
        public Task<SingleObjectResponse<Customer>> AddCustomer(Customer c);
        public Task<decimal> GenerateMedicalBill(int customerId, Dictionary<string, int> orders, decimal claim, int branchId);
        public Task ExecuteOrderAsync(Customer c, Dictionary<string, int> orders, decimal bill, int branchId);
    }
}
