namespace AutoMed_Backend.Interfaces
{
    public interface IInventoryLogic
    {
        public void PlaceOrder(string name, int qty);
        public void GetInventoryDetails();
        public decimal GetCashBalance();
        public decimal GetTotalSales();
    }
}
