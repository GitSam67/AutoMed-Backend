using AutoMed_Backend.Interfaces;
using AutoMed_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.Json;

namespace AutoMed_Backend.Repositories
{
    public class AdminLogic : IMedicineLogic, IInventoryLogic, ISalesLogic
    {
        StoreDbContext ctx;

        CollectionResponse<Medicine> collection;
        SingleObjectResponse<Medicine> single;

        public AdminLogic(StoreDbContext ctx, CollectionResponse<Medicine> coll, SingleObjectResponse<Medicine> single) 
        {
            this.ctx = ctx;
            this.collection = coll;
            this.single = single;
        }


        public async Task<CollectionResponse<Medicine>> GetMedicinesList(){

            try
            {
                var _result = await ctx.Medicines.ToListAsync();
                collection.Records = _result;
                collection.Message = "Medicines Read successfully..!!";
                collection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                collection.Message = ex.Message;
                collection.StatusCode = 500;
            }

            return collection;
        }

        public async Task<SingleObjectResponse<Medicine>> AddMedicine(Medicine med)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _medDb = ctx.Medicines.ToList();
                    var _medNames = (from m in _medDb select m.Name).ToList();
                    if (_medNames.Contains(med.Name.ToLower(), StringComparer.OrdinalIgnoreCase))
                    {
                        throw new Exception($"\nMedicine with name: '{med.Name}' already exists..!! Try again.");
                    }
                    else
                    {
                        var result = await ctx.Medicines.AddAsync(med);
                        await ctx.SaveChangesAsync();

                        foreach (var branchId in new[] { 1, 2, 3, 4 })
                        {
                            ctx.Inventory.Add(new Inventory() { MedicineId = med.MedicineId, Quantity = 0, BranchId = branchId });
                        }
                        await ctx.SaveChangesAsync();

                        single.Record = result.Entity;
                        single.Message = "Medicine added successfully..!!";
                        single.StatusCode = 200;
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    single.Message = ex.Message;
                    single.StatusCode = 500;
                }
            }
            return single;
        }

        public async Task<SingleObjectResponse<Medicine>> UpdateMedicine(int id, Medicine med)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.Medicines.FindAsync(id);
                    if (record != null)
                    {
                        record.Name = med.Name;
                        record.UnitPrice = med.UnitPrice;
                        record.ExpiryDate = med.ExpiryDate;
                        record.BatchNumber = med.BatchNumber;
                        record.Manufacturer = med.Manufacturer;
                        record.Category = med.Category;

                        await ctx.SaveChangesAsync();

                        single.Record = record;
                        single.Message = "Medicine Record is updated successfully";
                        single.StatusCode = 200;
                        await transaction.CommitAsync();

                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    single.Message = ex.Message;
                    single.StatusCode = 500;
                }
            }
            return single;
        }

        public async Task<SingleObjectResponse<Medicine>> RemoveMedicine(int id)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.Medicines.FindAsync(id);
                    if (record != null)
                    {
                        var name = record.Name;
                        ctx.Medicines.Remove(record);
                        await ctx.SaveChangesAsync();

                        single.Message = $"Medicine: '{name}' deleted successfully...!!";
                        single.StatusCode = 200;
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    single.Message = ex.Message;
                    single.StatusCode = 500;
                }
            }
            return single;
        }

        public async void PlaceOrder(Dictionary<Medicine, int> orders, string branchName)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var medicine = ctx.Medicines.ToList();
                    var inventory = ctx.Inventory.ToList();
                    var cashbalance = ctx.CashBalance.ToList();
                    bool flag = false;
                    foreach (var item in orders)
                    {

                        var med = from m in medicine where m.Name.ToLower() == item.Key.Name.ToLower() select new { id = m.MedicineId, price = m.UnitPrice };
                        var branch = ctx.Branches.Where(b => b.BranchName.ToLower().Equals(branchName.ToLower())).FirstOrDefault();
                        
                        foreach (var m in med)
                        {
                            var id = (from i in inventory where i.BranchId.Equals(branch.BranchId) && i.MedicineId.Equals(m.id) select i.InventoryId).FirstOrDefault();
                            var bal = cashbalance.FirstOrDefault();

                            var record = await ctx.Inventory.FindAsync(id);
                            if (record != null)
                            {
                                record.Quantity += item.Value;
                                bal.Balance -= (m.price - m.price / 2) * item.Value;

                                Console.WriteLine("\nOrder placed successfully..!!");
                                await ctx.SaveChangesAsync();
                                await transaction.CommitAsync();
                                flag = true;
                            }
                            else
                            {
                                Console.WriteLine("\nNo record found..!!");
                            }

                        }
                        if (!flag)
                        {
                            Console.WriteLine($"\nNo medicine with name: '{item.Key.Name}' exists...!! Try again.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<Dictionary<Medicine, int>> GetInventoryDetails()
        {
           var _result = new Dictionary<Medicine, int>();
            try
            {
                var inventory = await ctx.Inventory.ToListAsync();
                var medicines = await ctx.Medicines.ToListAsync();
                foreach (var stock in inventory)
                {
                    var med = (from m in medicines where m.MedicineId == stock.MedicineId select m).FirstOrDefault();
                    //Console.WriteLine($"*  Medicine: {med}, Qty: {stock.Quantity}");
                    if (med != null)
                    {
                        _result.Add(med, stock.Quantity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _result;
        }

        public decimal GetCashBalance()
        {
            var bal = ctx.CashBalance.FirstOrDefault();
            return bal.Balance;
        }
        public decimal GetTotalSales()
        {
            var bal = ctx.CashBalance.FirstOrDefault();
            return bal.TotalSales;
        }


        public async void CreateSalesReport(Customer c, Dictionary<string, int> orders, decimal bill)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    Orders sales = new Orders();
                    var _med = ctx.Medicines.ToList();

                    sales.CustomerId = c.CustomerId;
                    sales.PurchaseTime = DateTime.Now.Date;

                    foreach (var order in orders)
                    {

                        var med = (from m in _med where m.Name.ToLower() == order.Key.ToLower() select m).FirstOrDefault();

                        if (med != null)
                        {
                            sales.Medicines.Add(med);
                        }
                    }

                    sales.TotalBill = bill;

                    ctx.Orders.Add(sales);
                    await ctx.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }


        public async void GenerateSaleReport(Customer c, Dictionary<string, int> orders, decimal bill, string mode)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _med = ctx.Medicines.ToList();
                    var _sales = ctx.Orders.ToList();

                    var _sale = (from s in _sales where s.CustomerId == c.CustomerId select s).FirstOrDefault();

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "Sales_Report");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string filename = $"AutoMed_Sales_Report.txt";
                    string filepath = Path.Combine(folder, filename);

                    using (StreamWriter writer = new StreamWriter(filepath, true))
                    {
                        if (new FileInfo(filepath).Length == 0)
                        {
                            writer.WriteLine("\t\t\t\t\t\t\t * AutoMed Sales Report * ");
                            writer.WriteLine("\t\t\t\t\t\t\t   ----------------------   ");
                        }

                        writer.WriteLine("__________________________________________________________________________________________________________________________________________\n");
                        writer.WriteLine($"* Sale Id: {_sale.OrderId}\n");
                        writer.WriteLine($"  • Customer Name: {c.CustomerName}");
                        writer.WriteLine($"  • Date of purchase: {DateTime.Now.ToString("dd-MM-yyyy")}");
                        writer.WriteLine($"  • Payment mode: {mode}");
                        writer.WriteLine($"  • Products Purchased: ");

                        foreach (var order in orders)
                        {
                            var med = (from m in _med where order.Key.ToLower() == m.Name.ToLower() select m).FirstOrDefault();

                            if (med != null)
                            {
                                writer.WriteLine($"   - Medicine: {med.Name}, Qty: {order.Value}, Amount: ₹{med.UnitPrice}");
                            }
                        }

                        writer.WriteLine($"\n  • Total Sales Amount: ₹{bill}");

                        writer.WriteLine("\n__________________________________________________________________________________________________________________________________________");

                        writer.WriteLine($"\n\n* Total Gross Sales = ₹{GetTotalSales()}");
                        writer.WriteLine($"                      ---------- \n");
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
