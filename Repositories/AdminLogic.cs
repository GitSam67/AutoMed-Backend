﻿using AutoMed_Backend.Interfaces;
using AutoMed_Backend.Models;
using AutoMed_Backend.SecurityInfra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.Json;

namespace AutoMed_Backend.Repositories
{
    public class AdminLogic : IMedicineLogic, IInventoryLogic, ISalesLogic
    {
        StoreDbContext ctx;
        SecurityManagement security;

        CollectionResponse<Medicine> collection = new CollectionResponse<Medicine>();
        SingleObjectResponse<Medicine> singleMed = new SingleObjectResponse<Medicine>();
        CollectionResponse<Orders> orderCollection = new CollectionResponse<Orders>();
        CollectionResponse<Branch> branchCollection = new CollectionResponse<Branch>();
        SingleObjectResponse<Medicine> single = new SingleObjectResponse<Medicine>();
        SingleObjectResponse<StoreOwner> storeSingle = new SingleObjectResponse<StoreOwner>();
        CollectionResponse<StoreOwner> storeCollection = new CollectionResponse<StoreOwner>();
        SingleObjectResponse<Branch> branchSingle = new SingleObjectResponse<Branch>();
        CollectionResponse<Inventory> invCollection = new CollectionResponse<Inventory>();

        public AdminLogic(StoreDbContext ctx, SecurityManagement security) 
        {
            this.ctx = ctx;
            this.security = security;
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

        public async Task<SingleObjectResponse<Medicine>> GetMedicine(int medId)
        {

            try
            {
                var _result = await ctx.Medicines.Where(m => m.MedicineId == medId).FirstOrDefaultAsync();
                singleMed.Record = _result;
                singleMed.Message = "Medicines Read successfully..!!";
                singleMed.StatusCode = 200;
            }
            catch (Exception ex)
            {
                singleMed.Message = ex.Message;
                singleMed.StatusCode = 500;
            }

            return singleMed;
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

                        var branches = await ctx.Branches.ToListAsync();
                        var branchIds = (from b in branches select b.BranchId).ToList();

                        foreach (var branchId in branchIds)
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
                    var record = await ctx.StoreOwners.ToListAsync();
                    var isExisting = (from st in record where st.Email.Equals(o.Email) select true).FirstOrDefault();
                    if (isExisting)
                    {
                        storeSingle.Message = $"StoreOwner with email:'{o.Email}' already exists..!!";
                        storeSingle.StatusCode = 400;
                    }
                    else
                    {
                        var _result = await ctx.StoreOwners.AddAsync(o);
                        await ctx.SaveChangesAsync();

                        storeSingle.Record = _result.Entity;
                        storeSingle.Message = "StoreOwner added successfully..!!";
                        storeSingle.StatusCode = 200;

                        var user = new AppUser()
                        {
                            Name = o.OwnerName,
                            Email = o.Email,
                            Role = "StoreOwner",
                            Password = $"{o.OwnerName}@67",
                            ConfirmPassword = $"{o.OwnerName}@67"
                        };

                        await security.RegisterUserAsync(user);

                        await transaction.CommitAsync();
                    }
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

        public async Task<SingleObjectResponse<StoreOwner>> EditStoreOwner(int id, StoreOwner o)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.StoreOwners.FindAsync(id);
                    if (record != null)
                    {
                        record.OwnerName = o.OwnerName;
                        record.Email = o.Email;
                        record.BranchId = o.BranchId;

                        await ctx.SaveChangesAsync();

                        storeSingle.Record = record;
                        storeSingle.Message = "Store Owner Record is updated successfully";
                        storeSingle.StatusCode = 200;
                        await transaction.CommitAsync();

                    }
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

        public async Task<SingleObjectResponse<StoreOwner>> DeleteStoreOwner(int id)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.StoreOwners.FindAsync(id);
                    if (record != null)
                    {
                        var name = record.OwnerName;
                        ctx.StoreOwners.Remove(record);
                        await ctx.SaveChangesAsync();

                        storeSingle.Message = $"Store Owner: '{name}' deleted successfully...!!";
                        storeSingle.StatusCode = 200;
                        await transaction.CommitAsync();
                    }
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

                    var meds = await ctx.Medicines.ToListAsync();
                    Inventory inv = new Inventory() { BranchId = 0, Quantity = 0};

                    foreach (var m in meds)
                    {
                        ctx.Inventory.Add(new Inventory() { MedicineId = m.MedicineId, Quantity = 0, BranchId = b.BranchId });
                    }
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

        public async Task<SingleObjectResponse<Branch>> UpdateBranch(int id, Branch br)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.Branches.FindAsync(id);
                    if (record != null)
                    {
                        record.BranchName = br.BranchName;
                        record.Address = br.Address;

                        await ctx.SaveChangesAsync();

                        branchSingle.Record = record;
                        branchSingle.Message = "Branch Record is updated successfully";
                        branchSingle.StatusCode = 200;
                        await transaction.CommitAsync();

                    }
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

        public async Task<SingleObjectResponse<Branch>> DeleteBranch(int id)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.Branches.FindAsync(id);
                    if (record != null)
                    {
                        var name = record.BranchName;
                        ctx.Branches.Remove(record);
                        await ctx.SaveChangesAsync();

                        branchSingle.Message = $"Branch: '{name}' deleted successfully...!!";
                        branchSingle.StatusCode = 200;
                        await transaction.CommitAsync();

                    }
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

                        var med = (from m in medicine where m.Name.ToLower() == item.Key.ToLower() select m).ToList();

                        foreach (var m in med)
                        {
                            var id = (from i in inventory where i.BranchId.Equals(branchId) && i.MedicineId.Equals(m.MedicineId) select i.InventoryId).FirstOrDefault();
                            var bal = cashbalance.Where(c => c.BranchId.Equals(branchId)).FirstOrDefault();
                            
                            var record = await ctx.Inventory.FindAsync(id);
                            if (record != null)
                            {
                                var medRecord = await ctx.Medicines.FindAsync(m.MedicineId);
                                medRecord.Name = m.Name;
                                medRecord.UnitPrice = m.UnitPrice;
                                medRecord.BatchNumber = m.BatchNumber;
                                medRecord.Manufacturer = m.Manufacturer;
                                medRecord.Category = m.Category;
                                medRecord.ExpiryDate = DateTime.Now.AddMonths(12);
                                await ctx.SaveChangesAsync();

                                record.Quantity += item.Value;
                                await ctx.SaveChangesAsync();

                                bal.Balance -= (m.UnitPrice - m.UnitPrice / 2) * item.Value;
                                await ctx.SaveChangesAsync();
                                Console.WriteLine("\nOrder placed successfully..!!");
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

        public async Task<SingleObjectResponse<Medicine>> RemoveStock([FromBody] List<string> items, int branchId)
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

                        var med = from m in medicine where m.Name.ToLower() == item.ToLower() select new { id = m.MedicineId, price = m.UnitPrice };

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
                            Console.WriteLine($"\nNo medicine with name: '{item}' exists...!! Try again.");
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


        public async Task<CollectionResponse<Inventory>> GetInventoryDetails(int branchId)
        {
            try
            {
                var inventory = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();
                invCollection.Records = inventory;
                invCollection.Message = "Inventory details read successfully";
                invCollection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                invCollection.Message = "Inventory details fetching failed";
                invCollection.StatusCode = 500;
                throw ex;
            }

            return invCollection;
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

        public async Task<CollectionResponse<Branch>> GetBranches()
        {
            try
            {
                var _result = await ctx.Branches.ToListAsync();
                branchCollection.Records = _result;
                branchCollection.Message = "Branches Read successfully..!!";
                branchCollection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                branchCollection.Message = ex.Message;
                branchCollection.StatusCode = 500;
            }

            return branchCollection;
        }

        public async Task<CollectionResponse<StoreOwner>> GetStoreOwners()
        {
            try
            {
                var _result = await ctx.StoreOwners.ToListAsync();
                storeCollection.Records = _result;
                storeCollection.Message = "Store Owners Read successfully..!!";
                storeCollection.StatusCode = 200;
            }
            catch (Exception ex)
            {
                storeCollection.Message = ex.Message;
                storeCollection.StatusCode = 500;
            }

            return storeCollection;
        }

        public async Task<SingleObjectResponse<StoreOwner>> GetStoreOwner(int ownerId)
        {
            try
            {
                var _res = await ctx.StoreOwners.Where(s => s.OwnerId == ownerId).FirstOrDefaultAsync();
                storeSingle.Record = _res;
                storeSingle.Message = "Store Owners Read successfully..!!";
                storeSingle.StatusCode = 200;
            }
            catch (Exception ex)
            {
                storeSingle.Message = ex.Message;
                storeSingle.StatusCode = 500;
            }

            return storeSingle;
        }


        public async Task<bool> CreateSalesReportAsync(Customer c, Dictionary<string, int> orders, decimal bill, int branchId)
        {
            var isCreated = false;
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    Orders sales = new Orders();
                    var _med = await ctx.Medicines.ToListAsync();
                    
                    sales.CustomerId = c.CustomerId;
                    sales.PurchaseTime = DateTime.Now.Date;

                    foreach (var order in orders)
                    {
                        var med = (from m in _med where m.Name.ToLower() == order.Key.ToLower() select m).FirstOrDefault();

                        if (med != null)
                        {
                            sales.Medicines.Add(med);
                            sales.orders.Add(med.Name);
                        }
                    }

                    sales.TotalBill = bill;
                    sales.BranchId = branchId;

                    await ctx.Orders.AddAsync(sales);
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
                orderCollection.Message = "Sales Read successfully..!!";
                orderCollection.StatusCode = 200;
            
            }
            catch (Exception ex)
            {
                orderCollection.Message = ex.Message;
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
                    var x = (from i in _inv where i.MedicineId == m.MedicineId && m.ExpiryDate <= DateTime.Now.AddDays(10) select m).FirstOrDefault();
                    if (x != null)
                    {
                        result.Add(x);
                    }
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
                    var x = (from i in _inv where i.MedicineId == m.MedicineId && i.Quantity <= 5 select m).FirstOrDefault();
                    if (x != null)
                    {
                        result.Add(x);
                    }
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
