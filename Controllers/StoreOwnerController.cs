using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreOwnerController : ControllerBase
    {
        AdminLogic AdminLogic;
        CustomerLogic CustomerLogic;

        public StoreOwnerController(AdminLogic adminLogic, CustomerLogic customerLogic) 
        {
            this.AdminLogic = adminLogic; 
            this.CustomerLogic = customerLogic;
        }

        [HttpPost]
        [ActionName("AddCustomer")]
        public async Task<IActionResult> AddCustomer(Customer c)
        {
           var response = await CustomerLogic.AddCustomer(c);
              
            if (response.StatusCode.Equals(200))
            {
               response.Message = $"Customer {c.CustomerName} added successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost]
        [ActionName("AddMedicine")]
        public async Task<IActionResult> AddMedicine(Medicine med)
        {
            var response = await AdminLogic.AddMedicine(med);
            
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Medicine {med.Name} added successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [ActionName("UpdateMedicine")]
        public async Task<IActionResult> UpdateMedicine(int id, Medicine med)
        {
            var response = await AdminLogic.UpdateMedicine(id, med);
            
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Medicine {med.Name} updated successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete]
        [ActionName("RemoveMedicine")]
        public async Task<IActionResult> RemoveMedicine(int id)
        {
            var response = await AdminLogic.RemoveMedicine(id);
            
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Medicine removed successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [ActionName("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(Dictionary<Medicine, int> orders, int branchId)
        {
            var response = await AdminLogic.PlaceOrder(orders, branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Order placed successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetInventory")]
        public async Task<IActionResult> GetInventoryStock(int branchId)
        {
            var response = await AdminLogic.GetInventoryDetails(branchId);
            if (response.Count > 0)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetCashBalance")]
        public async Task<IActionResult> GetCashBalance(int branchId)
        {
            var response = AdminLogic.GetCashBalance(branchId);
     
            return Ok(response);
        }

        [HttpGet]
        [ActionName("GetTotalSales")]
        public async Task<IActionResult> GetTotalSales(int branchId)
        {
            var response = AdminLogic.GetTotalSales(branchId);

            return Ok(response);
        }

        [HttpPost]
        [ActionName("CreateSalesReport")]
        public async Task<IActionResult> CreateSalesReport(Customer c, Dictionary<Medicine, int> orders, decimal bill,int branchId)
        {
            var response = await AdminLogic.CreateSalesReport(c, orders, bill,branchId);
            if (response)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport(Customer c, Dictionary<Medicine, int> orders, decimal bill, string mode, string branchName)
        {
            var response = AdminLogic.GenerateSaleReport(c, orders, bill, mode, branchName);
            
            return Ok(response);
        }

    }
}
