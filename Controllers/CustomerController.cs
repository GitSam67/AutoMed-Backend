﻿using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
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

        [HttpPost("{branchId}")]
        [ActionName("CheckAvailableMedicines")]
        public async Task<IActionResult> CheckAvailableMedicines(int branchId)
        {
            var response = await CustomerLogic.CheckAvailability(branchId);

            return Ok(response);
         
        }

        [HttpPost("{customerId}")]
        [ActionName("GenerateMedicalBill")]
        public async Task<IActionResult> GenerateMedicalBill(int customerId, Dictionary<string, int> orders, decimal claim, int branchId)
        {
            var response = await CustomerLogic.GenerateMedicalBill(customerId, orders, claim, branchId);
            
            return Ok(response);
            
        }

        [HttpPost("{customerId}")]
        [ActionName("ViewMedicalBill")]
        public async Task<IActionResult> ViewMedicalBill(int customerId)
        {
            CustomerLogic.ViewMedicalBill(customerId);
            return Ok();
        }
    }
}
