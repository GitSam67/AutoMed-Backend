using System.Diagnostics;
using System.Linq;
using AutoMed_Backend.Interfaces;
using AutoMed_Backend.Models;
using Microsoft.EntityFrameworkCore;


namespace AutoMed_Backend.Repositories
{
    public class CustomerLogic : ICustomerLogic, ISalesLogic
    {

        StoreDbContext ctx;

        SingleObjectResponse<Customer> single = new SingleObjectResponse<Customer>();

        AdminLogic adminLogic;

        public CustomerLogic(StoreDbContext ctx, AdminLogic logic)
        {
            this.ctx = ctx;
            adminLogic = logic;
        }
        public async Task<SingleObjectResponse<Customer>> AddCustomer(Customer c)
        {

            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var record = await ctx.Customers.ToListAsync();
                    var isExisting = (from cust in record where cust.Email.Equals(c.Email) select true).FirstOrDefault();
                    if (isExisting)
                    {
                        single.Message = $"Customer with email:'{c.Email}' already exists..!!";
                        single.StatusCode = 400;
                    }
                    else
                    {
                        var _result = await ctx.Customers.AddAsync(c);
                        await ctx.SaveChangesAsync();

                        single.Record = _result.Entity;
                        single.Message = "Customer added successfully..!!";
                        single.StatusCode = 200;

                        await transaction.CommitAsync();
                        Console.WriteLine("\n\nYour details saved successfully...Please wait a moment your bill is being generated\n.");
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

        public async Task<Dictionary<string, int>> CheckAvailability(int branchId) 
        {
            var medDb = await ctx.Medicines.ToListAsync();
            var invDb = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();

            Dictionary<string, int> result = new Dictionary<string, int>();
            var _inv = await ctx.Inventory.Where(i => i.BranchId.Equals(branchId)).ToListAsync();
            var _med = await ctx.Medicines.ToListAsync();
            foreach (var i in _inv)
            {
                var x = (from m in _med where i.MedicineId == m.MedicineId select new { Med = m.Name, Qty = i.Quantity}).FirstOrDefault();
                result.Add(x.Med, x.Qty);
            }


            return result;
        }

        public async Task<decimal> GenerateMedicalBill(int customerId, Dictionary<string, int> orders, decimal claim, int branchId)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    decimal bill = 0;
                    int sr = 1;
                    decimal total = 0;
                    Random random = new Random();
                    var invoice = random.Next(1000000, 10000000);

                    var medicines = await ctx.Medicines.ToListAsync();
                    var c = ctx.Customers.Where(c => c.CustomerId.Equals(customerId)).FirstOrDefault();

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "Medical_Bills");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string filename = $"{c.CustomerName}_0{c.CustomerId}_MedBill.txt";
                    string filepath = Path.Combine(folder, filename);

                    using (StreamWriter writer = new StreamWriter(filepath))
                    {

                        writer.WriteLine("\t\t\t\t\t\t\t * AutoMed Invoice * ");
                        writer.WriteLine("\t\t\t\t\t\t\t   -----------------   ");

                        writer.WriteLine($"Date: {DateTime.Now}");
                        writer.WriteLine($"Invoice #: {invoice}");

                        writer.WriteLine("__________________________________________________________________________________________________________________________________________\n");

                        writer.WriteLine($"• Patient Details:\n");
                        writer.WriteLine($"  - Name: {c.CustomerName}");
                        writer.WriteLine($"  - Age: {c.Age}");
                        writer.WriteLine($"  - Gender: {c.Gender}");
                        writer.WriteLine($"  - BloodGroup: {c.BloodGroup}");
                        writer.WriteLine($"  - Contact no: +{c.ContactNo}");
                        writer.WriteLine($"  - Dr's Prescription: {c.Prescription}");

                        writer.WriteLine("__________________________________________________________________________________________________________________________________________");
                        writer.WriteLine("Sr No. |        Particulars              |      Qty        |       Amount         |");
                        writer.WriteLine("-----------------------------------------------------------------------------------");

                        foreach (var order in orders)
                        {

                            var med = (from m in medicines where m.Name.ToLower() == order.Key.ToLower() select m).FirstOrDefault();

                            writer.WriteLine($"  {sr++}    |    {med.Name} - {med.BatchNumber}        |      {order.Value}       |      {med.UnitPrice}        |");

                            total += med.UnitPrice * order.Value;
                        }

                        var _tax = total * 15 / 100;
                        var _net = total + _tax + 50;
                        var _afterclaim = _net - claim;

                        writer.WriteLine("                                      -----------------------------------------------");
                        writer.WriteLine($"                                                Total       |       {total}       ");
                        writer.WriteLine($"                                           Delivery Charge  |        + {50}       ");
                        writer.WriteLine($"                                                Tax (15%)   |      + {_tax}       ");
                        writer.WriteLine($"                                               Net Amount = |       {_net}        ");
                        writer.WriteLine($"                                                Med Claim   |      - {claim}      ");
                        writer.WriteLine("\n                                      -----------------------------------------------");
                        writer.WriteLine($"                                           *  Grand Total   |      ₹{_afterclaim} -/     |");
                        writer.WriteLine("__________________________________________________________________________________________________________________________________________");

                        bill = _afterclaim;
                    }


                    await transaction.CommitAsync();
                    Console.WriteLine($"\n\nYour bill amount of Rs.{bill} is generated.");
                    Console.WriteLine("\nNow, please pay your bill after proper reviewing the invoice sent.\n");
                    
                    await ExecuteOrderAsync(c, orders, bill, branchId);
                    
                    return bill;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        public void ViewMedicalBill(int customerId)
        {
            try
            {
                var c = ctx.Customers.Where(c => c.CustomerId.Equals(customerId)).FirstOrDefault();

                string folder = Path.Combine(Directory.GetCurrentDirectory(), "Medical_Bills");
                string filename = $"{c.CustomerName}_0{c.CustomerId}_MedBill.txt";
                string filepath = Path.Combine(folder, filename);

                if (File.Exists(filepath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filepath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("The specified file does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ExecuteOrderAsync(Customer c, Dictionary<string, int> orders, decimal bill, int branchId)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var medicines = await ctx.Medicines.ToListAsync();
                    var inventory = await ctx.Inventory.ToListAsync();
                    var branch = await ctx.Branches.FindAsync(branchId);

                    foreach (var order in orders)
                    {
                        var med = medicines.FirstOrDefault(m => m.Name.ToLower() == order.Key.ToLower());
                        if (med != null)
                        {
                            var inventoryRecord = inventory.FirstOrDefault(i => i.MedicineId == med.MedicineId && i.BranchId == branch.BranchId);
                            if (inventoryRecord != null)
                            {
                                inventoryRecord.Quantity -= order.Value;
                            }
                            else
                            {
                                Console.WriteLine("\nNo inventory record found..!!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\nNo medicine with name '{order.Key}' found..!!");
                        }
                    }

                    await ctx.SaveChangesAsync();

                    var cash = await ctx.CashBalance.FirstOrDefaultAsync(c => c.BranchId == branchId);
                    if (cash != null)
                    {
                        cash.Balance += bill;
                        cash.TotalSales += bill;
                        await ctx.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    Console.WriteLine("\n\nOrder executed successfully..!!");

                    await adminLogic.CreateSalesReportAsync(c, orders, bill, branch.BranchId);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        //public async void ExecuteOrder(Customer c, Dictionary<string, int> orders, decimal bill, int branchId)
        //{
        //    using (var transaction = ctx.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var medicine = await ctx.Medicines.ToListAsync();
        //            var inventory = await ctx.Inventory.ToListAsync(); 
        //            var branch = await ctx.Branches.FindAsync(branchId);

        //            foreach (var order in orders)
        //            {

        //                var med = (from m in medicine where m.Name.ToLower() == order.Key.ToLower() select new { id = m.MedicineId, price = m.UnitPrice }).FirstOrDefault();

        //                var id = (from i in inventory where i.MedicineId.Equals(med.id) && i.BranchId.Equals(branch.BranchId) select i.InventoryId).FirstOrDefault();
        //                var record = await ctx.Inventory.FindAsync(id);

        //                if (record != null)
        //                {
        //                    record.Quantity -= order.Value;
        //                }
        //                else
        //                {
        //                    Console.WriteLine("\nNo record found..!!");
        //                }

        //            }

        //            await ctx.SaveChangesAsync();

        //            var cash = await ctx.CashBalance.Where(c => c.BranchId == branchId).FirstOrDefaultAsync();
        //            cash.Balance += bill;
        //            cash.TotalSales += bill;
        //            await ctx.SaveChangesAsync();

        //            await transaction.CommitAsync();
        //            Console.WriteLine("\n\nOrder executed successfully..!!");

        //            await adminLogic.CreateSalesReport(c, orders, bill, branch.BranchId);
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //}

        public decimal GetTotalSales()
        {
            var bal = ctx.CashBalance.FirstOrDefault();
            return bal.TotalSales;
        }

        public async Task<object> GenerateSaleReport(Customer c, Dictionary<string, int> orders, decimal bill, string mode, string branchName)
        {
            object result = new object();
            List<object> ord = new List<object>();
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _med = await ctx.Medicines.ToListAsync();

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "Payment_Receipts");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string filename = $"{c.CustomerName}_0{c.CustomerId}_Pay_Receipt.txt";
                    string filepath = Path.Combine(folder, filename);

                    using (StreamWriter writer = new StreamWriter(filepath))
                    {

                        writer.WriteLine("\t\t\t\t\t\t\t * Payment Receipt * ");
                        writer.WriteLine("\t\t\t\t\t\t\t   ---------------   \n\n");
                        writer.WriteLine($"  • Customer Name: {c.CustomerName}");
                        writer.WriteLine($"  • Date of purchase: {DateTime.Now.ToString("dd-MM-yyyy")}");
                        writer.WriteLine($"  • Mode of payment: {mode}");
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

                        writer.WriteLine($"\n  • Total Amount: ₹{bill}");
                        writer.WriteLine("                  ----------\n\n");

                        writer.WriteLine($"{new string(' ', 43)}Thank you for your purchase.. Have a good day...!!");

                    }

                    result = new
                    {
                        CustomerName = c.CustomerName,
                        PurchaseDate = DateTime.Now.ToString("dd-MM-yyyy"),
                        Mode = mode,
                        Orders = ord,
                        TotalAmount = bill,
                    };

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}
