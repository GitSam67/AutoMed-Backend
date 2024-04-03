using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ICustomerLogic
    {
        public Task<SingleObjectResponse<Customer>> AddCustomer(Customer c);
        public Task<decimal> GenerateMedicalBill(Customer c, Dictionary<Medicine, int> orders, decimal claim, string branchName);
        public void ExecuteOrder(Customer c, Dictionary<Medicine, int> orders, decimal bill, string branchName);
    }
}
