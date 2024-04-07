using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using AutoMed_Backend.SecurityInfra;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class StoreOwnerController : ControllerBase
    {
        SecurityManagement security;
        AdminLogic AdminLogic;
        CustomerLogic CustomerLogic;

        public StoreOwnerController(AdminLogic adminLogic, CustomerLogic customerLogic, SecurityManagement security) 
        {
            this.AdminLogic = adminLogic; 
            this.CustomerLogic = customerLogic;
            this.security = security;
        }

        [HttpPost]
        [ActionName("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer(AppUser user)
        {
            var response = await security.RegisterUserAsync(user);

            if (response)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost("{branchId}")]
        [ActionName("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] Dictionary<string, int> orders, int branchId)
        {
            var response = await AdminLogic.PlaceOrder(orders, branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Order placed successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{branchId}")]
        [ActionName("RemoveStock")]
        public async Task<IActionResult> RemoveStock(List<string> orders, int branchId)
        {
            var response = await AdminLogic.RemoveStock(orders, branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Stock removed successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("{branchId}")]
        [ActionName("GetInventory")]
        public async Task<IActionResult> GetInventoryStock(int branchId)
        {
            var response = await AdminLogic.GetInventoryDetails(branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Inventory details read successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{branchId}")]
        [ActionName("GetCashBalance")]
        public async Task<IActionResult> GetCashBalance(int branchId)
        {
            var response = AdminLogic.GetCashBalance(branchId);
     
            return Ok(response);
        }

        [HttpGet("{branchId}")]
        [ActionName("GetTotalSales")]
        public async Task<IActionResult> GetTotalSales(int branchId)
        {
            var response = AdminLogic.GetTotalSales(branchId);

            return Ok(response);
        }


        [HttpGet("{branchId}")]
        [ActionName("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport(int branchId)
        {
            var response = await AdminLogic.GetSalesReport(branchId);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Sales read successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{branchId}")]
        [ActionName("CheckForExpiry")]
        public async Task<IActionResult> CheckForExpiry(int branchId)
        {
            var response = await AdminLogic.CheckForExpiry(branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Expiring medicines..!!";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{branchId}")]
        [ActionName("CheckForStockLevel")]
        public async Task<IActionResult> CheckForStockLevel(int branchId)
        {
            var response = await AdminLogic.CheckForStockLevel(branchId);
            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Low stock medicines..!!";
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
