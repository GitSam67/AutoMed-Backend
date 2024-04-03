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
    public class SuperAdminController : ControllerBase
    {
        SecurityManagement security;
        SecurityResponse response;
        AdminLogic AdminLogic;

        public SuperAdminController(SecurityManagement security, AdminLogic adminLogic)
        {
            this.security = security;
            response = new SecurityResponse();
            AdminLogic = adminLogic;
        }


        [HttpPost]
        [ActionName("AddStoreOwner")]
        public async Task<IActionResult> AddStoreOwner(StoreOwner o)
        {
            var response = await AdminLogic.AddStoreOwner(o);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"StoreOwner {o.OwnerName} added successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetMedicine")]
        public async Task<IActionResult> GetMedicine()
        {
            var response = await AdminLogic.GetMedicinesList();

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Medicine read successfully";
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
        [ActionName("AddBranch")]
        public async Task<IActionResult> AddBranch(Branch branch)
        {
            var response = await AdminLogic.AddBranch(branch);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Branch {branch.BranchName} added successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport(int branchId)
        {
            var response = AdminLogic.GetSalesReport(branchId);

            //if (response.StatusCode.Equals(200))
            //{
            //    response.Message = $"Medicine read successfully";
            //    return Ok(response);
            //}
            //return BadRequest(response);
            return Ok(response);
        }

        //[HttpPost]
        //[ActionName("newrole")]
        //[Authorize(Policy = "AdminPolicy")]
        //public async Task<IActionResult> CreateRole(RoleInfo role)
        //{
        //    try
        //    {
        //        var result = await security.CreateRoleAsync(role);
        //        if (result)
        //        {
        //            response.Message = $"Role {role.Name} is created successfully";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //        throw ex;
        //    }
        //    return Ok(response);
        //}
    }
}
