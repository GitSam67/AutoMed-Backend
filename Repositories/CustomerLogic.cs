﻿using System.Diagnostics;
using System.Linq;
using AutoMed_Backend.Interfaces;
using AutoMed_Backend.Models;
using Microsoft.EntityFrameworkCore;


namespace AutoMed_Backend.Repositories
{
    public class CustomerLogic : ICustomerLogic, ISalesLogic
    {

        StoreDbContext ctx;

        SingleObjectResponse<Customer> single;

        AdminLogic adminLogic;

        public CustomerLogic(StoreDbContext ctx, SingleObjectResponse<Customer> _single, AdminLogic adminLogic)
        {
            this.ctx = ctx;
            single = _single;
            this.adminLogic = adminLogic;
        }
        public async Task<SingleObjectResponse<Customer>> AddCustomer(Customer c)
        {

            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _result = await ctx.Customers.AddAsync(c);
                    await ctx.SaveChangesAsync();

                    single.Record = _result.Entity;
                    single.Message = "Customer added successfully..!!";
                    single.StatusCode = 200;

                    await transaction.CommitAsync();
                    Console.WriteLine("\n\nYour details saved successfully...Please wait a moment your bill is being generated\n.");
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

        public async Task<KeyValuePair<Dictionary<Medicine, int>, List<string>>> CheckAvailability(Dictionary<Medicine, int> orders) 
        {
            List<string> NotAvailables = new List<string>();
            var medicineDb = await ctx.Medicines.Select(m => m.Name).ToListAsync();
            var medDb = await ctx.Medicines.ToListAsync();
            var invDb = await ctx.Inventory.ToListAsync();
            bool flag = false;

            foreach (var o in orders)
            {
                if (medicineDb.Contains(o.Key.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var id = (from m in medDb where m.Name.ToLower() == o.Key.Name.ToLower() select m.MedicineId).FirstOrDefault();
                    var inventory = (from i in invDb where i.MedicineId == id select i).FirstOrDefault();
                    if (inventory.Quantity - o.Value >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        NotAvailables.Add(o.Key.Name);
                        orders.Remove(o.Key);
                        flag = true;
                    }
                }
                else
                {
                    NotAvailables.Add(o.Key.Name);
                    orders.Remove(o.Key);
                    flag = true;
                }
            }

            return KeyValuePair.Create(orders, NotAvailables);
        }

        public async Task<decimal> GenerateMedicalBill(Customer c, Dictionary<Medicine, int> orders, decimal claim, string branchName)
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

                    var medicines = ctx.Medicines.ToList();

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

                            var med = (from m in medicines where m.Name.ToLower() == order.Key.Name.ToLower() select m).FirstOrDefault();

                            writer.WriteLine($"  {sr++}    |    {med.Name} - {med.BatchNumber}        |      {order.Value}       |      {med.UnitPrice}        |");

                            total += med.UnitPrice * order.Value;
                        }

                        var _tax = total * 15 / 100;
                        var _net = total + _tax;
                        var _afterclaim = _net - claim;

                        writer.WriteLine("                                      -----------------------------------------------");
                        writer.WriteLine($"                                                Total       |       {total}       ");
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
                    
                    ExecuteOrder(c, orders, bill, branchName);
                    
                    return bill;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        public void ViewMedicalBill(Customer c)
        {
            try
            {
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

        public async void ExecuteOrder(Customer c, Dictionary<Medicine, int> orders, decimal bill, string branchName)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var medicine = ctx.Medicines.ToList();
                    var inventory = ctx.Inventory.ToList();
                    var cash = (ctx.CashBalance.ToList()).FirstOrDefault();
                    var branch = ctx.Branches.Where(b => b.BranchName.ToLower().Equals(branchName.ToLower())).FirstOrDefault();

                    foreach (var order in orders)
                    {

                        // order.key -> Medicine name
                        // order.value -> qty

                        var med = (from m in medicine where m.Name.ToLower() == order.Key.Name.ToLower() select new { id = m.MedicineId, price = m.UnitPrice }).FirstOrDefault();

                        var id = (from i in inventory where i.MedicineId.Equals(med.id) && i.BranchId.Equals(branch.BranchId) select i.InventoryId).FirstOrDefault();
                        var record = await ctx.Inventory.FindAsync(id);

                        if (record != null)
                        {
                            record.Quantity -= order.Value;
                        }
                        else
                        {
                            Console.WriteLine("\nNo record found..!!");
                        }

                    }

                    await ctx.SaveChangesAsync();

                    cash.Balance += bill;
                    cash.TotalSales += bill;
                    await ctx.SaveChangesAsync();

                    adminLogic.CreateSalesReport(c, orders, bill, branch.BranchId);

                    await transaction.CommitAsync();
                    Console.WriteLine("\n\nOrder executed successfully..!!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public decimal GetTotalSales()
        {
            var bal = ctx.CashBalance.FirstOrDefault();
            return bal.TotalSales;
        }

        public async Task<object> GenerateSaleReport(Customer c, Dictionary<Medicine, int> orders, decimal bill, string mode, string branchName)
        {
            object result = new object();
            List<object> ord = new List<object>();
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var _med = ctx.Medicines.ToList();

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
                            var med = (from m in _med where order.Key.Name.ToLower() == m.Name.ToLower() select m).FirstOrDefault();

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
