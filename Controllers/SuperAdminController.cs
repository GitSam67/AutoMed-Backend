using AutoMed_Backend.Models;
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

        public SuperAdminController(SecurityManagement security)
        {
            this.security = security;
            response = new SecurityResponse();
        }


        [HttpPost]
        [ActionName("newrole")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateRole(RoleInfo role)
        {
            try
            {
                var result = await security.CreateRoleAsync(role);
                if (result)
                {
                    response.Message = $"Role {role.Name} is created successfully";
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
