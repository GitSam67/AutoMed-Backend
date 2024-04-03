namespace AutoMed_Backend.Models
{
    public class CashBalance
    {
        public int id { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalSales { get; set; }
        public int BranchId { get; set; } 
    }
}
