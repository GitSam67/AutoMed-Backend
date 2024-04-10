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
    [Authorize]
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
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        [ActionName("EditStoreOwner")]
        public async Task<IActionResult> EditStoreOwner(int id, StoreOwner o)
        {
            var response = await AdminLogic.EditStoreOwner(id, o);

            if (response.StatusCode.Equals(200))
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ActionName("DeleteStoreOwner")]
        public async Task<IActionResult> DeleteStoreOwner(int id)
        {
            var response = await AdminLogic.DeleteStoreOwner(id);

            if (response.StatusCode.Equals(200))
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetStoreOwners")]
        public async Task<IActionResult> GetStoreOwners()
        {
            var response = await AdminLogic.GetStoreOwners();

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"StoreOwners read successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        [ActionName("GetStoreOwners")]
        public async Task<IActionResult> GetStoreOwner(int id)
        {
            var response = await AdminLogic.GetStoreOwner(id);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"StoreOwner read successfully";
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
                response.Message = $"Medicines read successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        [ActionName("GetMedicine")]
        public async Task<IActionResult> GetMedicine(int id)
        {
            var response = await AdminLogic.GetMedicine(id);

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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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

        [HttpPut("{id}")]
        [ActionName("UpdateBranch")]
        public async Task<IActionResult> UpdateBranch(int id, Branch branch)
        {
            var response = await AdminLogic.UpdateBranch(id, branch);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Branch {branch.BranchName} updated successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ActionName("DeleteBranch")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var response = await AdminLogic.DeleteBranch(id);

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Branch deleted successfully";
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ActionName("GetBranches")]
        public async Task<IActionResult> GetBranches()
        {
            var response = await AdminLogic.GetBranches();

            if (response.StatusCode.Equals(200))
            {
                response.Message = $"Branches read successfully";
                return Ok(response);
            }
            return BadRequest(response);
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
