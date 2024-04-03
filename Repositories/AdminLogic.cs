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

        CollectionResponse<Medicine> collection = new CollectionResponse<Medicine>();
        CollectionResponse<Orders> orderCollection = new CollectionResponse<Orders>();
        SingleObjectResponse<Medicine> single = new SingleObjectResponse<Medicine>();
        SingleObjectResponse<StoreOwner> storeSingle = new SingleObjectResponse<StoreOwner>();
        SingleObjectResponse<Branch> branchSingle = new SingleObjectResponse<Branch>();

        public AdminLogic(StoreDbContext ctx) 
        {
            this.ctx = ctx;
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

                        foreach (var branchId in new[] { 1, 2, 3, 4, 5 })
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

        public async Task<SingleObjectResponse<StoreOwner>> AddStoreOwner(StoreOwner o)
        {

            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _result = await ctx.StoreOwners.AddAsync(o);
                    await ctx.SaveChangesAsync();

                    storeSingle.Record = _result.Entity;
                    storeSingle.Message = "StoreOwner added successfully..!!";
                    storeSingle.StatusCode = 200;

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    storeSingle.Message = ex.Message;
                    storeSingle.StatusCode = 500;
                }
            }
            return storeSingle;
        }

        public async Task<SingleObjectResponse<Branch>> AddBranch(Branch b)
        {

            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _result = await ctx.Branches.AddAsync(b);
                    await ctx.SaveChangesAsync();

                    branchSingle.Record = _result.Entity;
                    branchSingle.Message = "Branch added successfully..!!";
                    branchSingle.StatusCode = 200;

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    branchSingle.Message = ex.Message;
                    branchSingle.StatusCode = 500;
                }
            }
            return branchSingle;
        }

        public async Task<SingleObjectResponse<Medicine>> PlaceOrder(Dictionary<string, int> orders, int branchId)
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

                        var med = from m in medicine where m.Name.ToLower() == item.Key.ToLower() select new { id = m.MedicineId, price = m.UnitPrice };
                        
                        foreach (var m in med)
                        {
                            var id = (from i in inventory where i.BranchId.Equals(branchId) && i.MedicineId.Equals(m.id) select i.InventoryId).FirstOrDefault();
                            var bal = cashbalance.Where(c => c.BranchId.Equals(branchId)).FirstOrDefault();

                            var record = await ctx.Inventory.FindAsync(id);
                            if (record != null)
                            {
                                record.Quantity += item.Value;
                                bal.Balance -= (m.price - m.price / 2) * item.Value;

                                Console.WriteLine("\nOrder placed successfully..!!");
                                await ctx.SaveChangesAsync();
                                await transaction.CommitAsync();

                                single.Message = $"Order placed successfully...!!";
                                single.StatusCode = 200;
                                flag = true;
                            }
                            else
                            {
                                Console.WriteLine("\nNo record found..!!");
                            }

                        }
                        if (!flag)
                        {
                            Console.WriteLine($"\nNo medicine with name: '{item.Key}' exists...!! Try again.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    single.Message = $"Failed to place order...!!";
                    single.StatusCode = 500;
                }
            }
            return single;
        }

        public async Task<SingleObjectResponse<Medicine>> RemoveStock(Dictionary<string, int> items, int branchId)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var medicine = ctx.Medicines.ToList();
                    var inventory = ctx.Inventory.ToList();
                    bool flag = false;
                    foreach (var item in items)
                    {

                        var med = from m in medicine where m.Name.ToLower() == item.Key.ToLower() select new { id = m.MedicineId, price = m.UnitPrice };

                        foreach (var m in med)
                        {
                            var id = (from i in inventory where i.BranchId.Equals(branchId) && i.MedicineId.Equals(m.id) select i.InventoryId).FirstOrDefault();
                            
                            var record = await ctx.Inventory.FindAsync(id);
                            if (record != null)
                            {
                                record.Quantity = 0;

                                Console.WriteLine("\nOrder removed successfully..!!");
                                await ctx.SaveChangesAsync();
                                await transaction.CommitAsync();

                                single.Message = $"Order removed successfully...!!";
                                single.StatusCode = 200;
                                flag = true;
                            }
                            else
                            {
                                Console.WriteLine("\nNo record found..!!");
                            }

                        }
                        if (!flag)
                        {
                            Console.WriteLine($"\nNo medicine with name: '{item.Key}' exists...!! Try again.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    single.Message = $"Failed to remove order...!!";
                    single.StatusCode = 500;
                }
            }
            return single;
        }


        public async Task<Dictionary<string, int>> GetInventoryDetails(int branchId)
        {
           var _result = new Dictionary<string, int>();
            try
            {
                var inventory = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();
                var medicines = await ctx.Medicines.ToListAsync();

                foreach (var stock in inventory)
                {
                    var med = (from m in medicines where m.MedicineId == stock.MedicineId select m).FirstOrDefault();
                    //Console.WriteLine($"*  Medicine: {med}, Qty: {stock.Quantity}");
                    if (med != null)
                    {
                        _result.Add(med.Name, stock.Quantity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _result;
        }

        public decimal GetCashBalance(int branchId)
        {
            var bal = ctx.CashBalance.Where(c => c.BranchId.Equals(branchId)).FirstOrDefault();
            return bal.Balance;
        }
        public decimal GetTotalSales(int branchId)
        {
            var bal = ctx.CashBalance.Where(c => c.BranchId.Equals(branchId)).FirstOrDefault();
            return bal.TotalSales;
        }


        public async Task<bool> CreateSalesReport(Customer c, Dictionary<string, int> orders, decimal bill, int branchId)
        {
            var isCreated = false;
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
                    sales.BranchId = branchId;

                    ctx.Orders.Add(sales);
                    await ctx.SaveChangesAsync();
                    await transaction.CommitAsync();
                    isCreated = true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;      
                }
            }
            return isCreated;
        }


        public async Task<CollectionResponse<Orders>> GetSalesReport(int branchId) 
        {
            try
            {
                if (branchId > 0)
                {
                    orderCollection.Records = await ctx.Orders.Where(s => s.BranchId.Equals(branchId)).ToListAsync();
                }
                else
                {
                    orderCollection.Records = await ctx.Orders.ToListAsync();
                }
                orderCollection.StatusCode = 200;
            
            }
            catch (Exception ex)
            {
                orderCollection.StatusCode = 500;
                throw ex;
            }

            return orderCollection;
        }
        

        public async Task<object> GenerateSaleReport(Customer c, Dictionary<string, int> orders, decimal bill, string mode, string branchName)
        {
            object result = new object();
            List<object> ord = new List<object>();
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _med = ctx.Medicines.ToList();
                    var _sales = ctx.Orders.ToList();
                    var branch = ctx.Branches.Where(b => b.BranchName.ToLower().Equals(branchName.ToLower())).FirstOrDefault();

                    var _sale = (from s in _sales where s.CustomerId == c.CustomerId && s.BranchId == branch.BranchId select s).FirstOrDefault();

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
                                ord.Add(new
                                {
                                    Medicine = med.Name,
                                    Qty = order.Value,
                                    Amount = med.UnitPrice
                                });
                            }
                        }

                        writer.WriteLine($"\n  • Total Sales Amount: ₹{bill}");

                        writer.WriteLine("\n__________________________________________________________________________________________________________________________________________");

                        writer.WriteLine($"\n\n* Total Gross Sales = ₹{GetTotalSales(branch.BranchId)}");
                        writer.WriteLine($"                      ---------- \n");
                    }

                    result = new
                    {
                        SaleId = _sale.OrderId,
                        CustomerName = c.CustomerName,
                        PurchaseDate = DateTime.Now.ToString("dd-MM-yyyy"),
                        Mode = mode,
                        Orders = ord,
                        SalesAmount = bill,
                    };

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
            return result;
        }

        public async Task<CollectionResponse<Medicine>> CheckForExpiry(int branchId) 
        {
            try
            {
                List<Medicine> result = new List<Medicine>();
                var _inv = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();
                var _med = await ctx.Medicines.ToListAsync();
                foreach (var m in _med)
                {
                    var x = (from i in _inv where i.InventoryId == m.MedicineId && m.ExpiryDate <= DateTime.Now.AddDays(10) select m).FirstOrDefault();
                    result.Add(x);
                }

                collection.Records = result;
                collection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                collection.StatusCode = 500;
                throw ex;
            }

            return collection;
        }

        public async Task<CollectionResponse<Medicine>> CheckForStockLevel(int branchId)
        {
            try
            {
                List<Medicine> result = new List<Medicine>();
                var _inv = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();
                var _med = await ctx.Medicines.ToListAsync();
                foreach (var m in _med)
                {
                    var x = (from i in _inv where i.InventoryId == m.MedicineId && i.Quantity <= 5 select m).FirstOrDefault();
                    result.Add(x);
                }

                collection.Records = result;
                collection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                collection.StatusCode = 500;
                throw ex;
            }

            return collection;
        }
    }
}
