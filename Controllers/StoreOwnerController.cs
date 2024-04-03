using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
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
        [ActionName("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(Dictionary<string, int> orders, int branchId)
        {
            var response = await AdminLogic.PlaceOrder(orders, branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Order placed successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete]
        [ActionName("RemoveStock")]
        public async Task<IActionResult> RemoveStock(Dictionary<string, int> orders, int branchId)
        {
            var response = await AdminLogic.RemoveStock(orders, branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Stock removed successfully";
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


        [HttpGet]
        [ActionName("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport(int branchId)
        {
            var response = AdminLogic.GetSalesReport(branchId);
            
            return Ok(response);
        }

        [HttpGet]
        [ActionName("CheckForExpiry")]
        public async Task<IActionResult> CheckForExpiry(int branchId)
        {
            var response = AdminLogic.CheckForExpiry(branchId);
           
            return Ok(response);
        }

        [HttpGet]
        [ActionName("CheckForStockLevel")]
        public async Task<IActionResult> CheckForStockLevel(int branchId)
        {
            var response = AdminLogic.CheckForStockLevel(branchId);

            return Ok(response);
        }

    }
}
