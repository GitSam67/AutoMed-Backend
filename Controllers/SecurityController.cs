using AutoMed_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMed_Backend.SecurityInfra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoMed_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        SecurityManagement security;
        SecurityResponse response;

        public SecurityController(SecurityManagement security)
        {
            this.security = security;
            response = new SecurityResponse();
        }

        [HttpPost]
        [ActionName("register")]
        public async Task<IActionResult> RegisterUserAsync(AppUser user, string branch)
        {
            try
            {
                var result = await security.RegisterUserAsync(user, branch);
                if (result)
                {
                    response.Message = $"User {user.Email} created successfully";

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                throw ex;
            }
            return Ok(response);
        }

        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> AuthenticateUserAsync(LoginUser user)
        {
            try
            {
                response = await security.AuthenticateUserAsync(user);

                if (response.IsLoggedIn)
                {
                    response.Message = $"User {user.Email} authenticated successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                throw ex;
            }
            return Ok(response);
        }


        [HttpGet]
        [ActionName("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUserDetail(string token)
        {
            var result = await security.GetUserFromTokenAsync(token);

            return Ok(result);
        }



        [HttpDelete]
        [ActionName("logout")]
        public async Task<IActionResult> Logout()
        {
            await security.LogoutAsync();
            return Ok("Logged out Successfully");
        }
    }
}
