using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ISalesLogic
    {
        public Task<object> GenerateSaleReport(Customer c, Dictionary<Medicine, int> orders, decimal bill, string mode, string branchName);

    }
}
