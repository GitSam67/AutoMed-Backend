using AutoMed_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AutoMed_Backend.SecurityInfra
{
    public class SecurityManagement
    {
        UserManager<IdentityUser> UserManager;
        SignInManager<IdentityUser> SignInManager;
        RoleManager<IdentityRole> RoleManager;
        StoreDbContext ctx;
        IConfiguration config;

        public SecurityManagement(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, StoreDbContext ctx, IConfiguration _config)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            RoleManager = roleManager;
            this.ctx = ctx;
            config = _config;
        }
        public async Task<bool> RegisterUserAsync(AppUser user, string branch)
        {
            bool isUserCreated = false;
            try
            {
                var userExist = await UserManager.FindByEmailAsync(user.Email);
                if (userExist != null)
                    throw new Exception($"User with Email: '{user.Email}' already exists..!!");


                IdentityUser newUser = new IdentityUser()
                {
                    UserName = user.Name,
                    Email = user.Email,
                };


                if (user.Password.Equals(user.ConfirmPassword))
                {
                    var result = await UserManager.CreateAsync(newUser, user.Password);
                    if (result.Succeeded)
                    {
                        isUserCreated = true;
                        await AssignRoleToUser(new UserRole { Email = user.Email, RoleName = user.Role });

                        if (user.Role.Equals("Customer"))
                        {

                            var c = new Customer()
                            {
                                CustomerName = user.Name,
                                Email = user.Email,
                                Password = user.Password
                            };

                            await ctx.Customers.AddAsync(c);
                            await ctx.SaveChangesAsync();
                        }
                        else if (user.Role.Equals("StoreOwner")) 
                        {
                            var b = ctx.Branches.Where(b => b.BranchName.ToLower().Equals(branch.ToLower())).FirstOrDefault();
                            var o = new StoreOwner()
                            {
                                Name = user.Name,
                                Email = user.Email,
                                Password = user.Password,
                                BranchId = b.BranchId
                            };

                            await ctx.StoreOwners.AddAsync(o);
                            await ctx.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    throw new Exception($"Passwords must be equal..!!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isUserCreated;
        }

        public async Task<SecurityResponse> AuthenticateUserAsync(LoginUser user)
        {
            SecurityResponse response = new SecurityResponse();

            try
            {
                var userExist = await UserManager.FindByEmailAsync(user.Email);
                if (userExist == null)
                    throw new Exception($"User with Email: '{user.Email}' not found");

                // 2. Authenticate the User
                var result = await SignInManager.PasswordSignInAsync(user.Email, user.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var secretKey = Convert.FromBase64String(config["JWTCoreSettings:SecretKey"]);

                    var expiry = Convert.ToInt32(config["JWTCoreSettings:ExpiryInMinutes"]);


                    IdentityUser idUser = new IdentityUser(user.Email);
                    var roles = await UserManager.GetRolesAsync(userExist);

                    if (roles == null || !roles.Any())
                        throw new Exception($"No role associated with user {user.Email}");

                    var securityTokenDescription = new SecurityTokenDescriptor()
                    {
                        Issuer = null,
                        Audience = null,
                        Subject = new ClaimsIdentity(new List<Claim>() {
                            new Claim("UserId", idUser.Id),
                            new Claim("UserRole", roles[0])
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(expiry),
                        IssuedAt = DateTime.UtcNow,
                        NotBefore = DateTime.UtcNow,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    // Generate the JSON Token
                    // Header.Payload.Signature
                    var jwtHandler = new JwtSecurityTokenHandler();

                    var jwtToken = jwtHandler.CreateJwtSecurityToken(securityTokenDescription);

                    var _roleName = jwtToken.Claims.Take(2).Last().Value;

                    response.Token = jwtHandler.WriteToken(jwtToken);
                    response.Role = _roleName;
                    response.IsLoggedIn = true;
                }
                else
                {
                    throw new Exception($"Login failed for User with Email {user.Email}, try once again..!!");
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public async Task<KeyValuePair<string, string>> GetUserFromTokenAsync(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = jwtHandler.ReadJwtToken(token);

            var userId = jwtSecurityToken.Claims.First().Value;

            var identityUser = await UserManager.FindByIdAsync(userId);

            var _userEmail = identityUser.Email;

            var _roleName = jwtSecurityToken.Claims.Take(2).Last().Value;

            return KeyValuePair.Create(_userEmail, _roleName);
        }

        public async Task<bool> CreateRoleAsync(RoleInfo role)
        {
            bool isCreated = false;
            try
            {
                var roleAvailable = await RoleManager.FindByNameAsync(role.Name);
                if (roleAvailable != null)
                    throw new Exception($"Role {role.Name} already exists..!!");


                IdentityRole identityRole = new IdentityRole()
                {
                    Name = role.Name,
                    NormalizedName = role.Name.ToUpper()
                };

                var result = await RoleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                    isCreated = true;
                else
                    throw new Exception($"Role {role.Name} creation failed");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isCreated;
        }


        public async Task<bool> AssignRoleToUser(UserRole userRole)
        {
            bool isRoleAssigned = false;
            try
            {
                var roleAvailable = await RoleManager.FindByNameAsync(userRole.RoleName);
                if (roleAvailable == null)
                    throw new Exception($"Role: {userRole.RoleName} not exists..!!");

                var userAvailable = await UserManager.FindByEmailAsync(userRole.Email);
                if (userAvailable == null)
                    throw new Exception($"User {userRole.Email} not exists..!!");

                var result = await UserManager.AddToRoleAsync(userAvailable, userRole.RoleName);
                if (result.Succeeded)
                    isRoleAssigned = true;
                else
                    throw new Exception($"Some error occurred while assigning Role : {userRole.RoleName} to User : {userRole.Email}");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isRoleAssigned;
        }

        public async Task<bool> LogoutAsync()
        {
            await SignInManager.SignOutAsync();
            return true;
        }
    }
}
