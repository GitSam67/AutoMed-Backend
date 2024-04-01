using AutoMed_Backend.Models;

namespace AutoMed_Backend.Interfaces
{
    public interface ISalesLogic
    {
        public void GenerateSaleReport(Customer c, Dictionary<string, int> orders, decimal bill, string mode);

    }
}
