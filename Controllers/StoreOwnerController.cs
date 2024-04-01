using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
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
        SecurityResponse response;

        public StoreOwnerController(AdminLogic adminLogic, CustomerLogic customerLogic) 
        {
            this.AdminLogic = adminLogic; 
            this.CustomerLogic = customerLogic;
            response = new SecurityResponse();
        }

        [HttpPost]
        [ActionName("addcustomer")]
        public async Task<IActionResult> AddCustomer(Customer c)
        {
            try
            {
                var result = await CustomerLogic.AddCustomer(c);
                if (result.StatusCode.Equals(200))
                {
                    response.Message = $"Customer {c.CustomerName} added successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                throw ex;
            }
            return Ok(response);
        }
    }
}
