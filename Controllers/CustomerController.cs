using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        AdminLogic AdminLogic;
        CustomerLogic CustomerLogic;

        public CustomerController(AdminLogic adminLogic, CustomerLogic customerLogic)
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
        [ActionName("CheckAvailableMedicines")]
        public async Task<IActionResult> CheckAvailableMedicines(int branchId)
        {
            var response = await CustomerLogic.CheckAvailability(branchId);

            return Ok(response);
         
        }

        [HttpPost]
        [ActionName("GenerateMedicalBill")]
        public async Task<IActionResult> GenerateMedicalBill(int customerId, Dictionary<string, int> orders, decimal claim, string branchName)
        {
            var response = await CustomerLogic.GenerateMedicalBill(customerId, orders, claim, branchName);

            return Ok(response);
        }

        [HttpPost]
        [ActionName("ViewMedicalBill")]
        public async Task<IActionResult> ViewMedicalBill(int customerId)
        {
            CustomerLogic.ViewMedicalBill(customerId);
            return Ok();
        }
    }
}
