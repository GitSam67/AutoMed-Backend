using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMed_Backend.Models
{
    public class AppSecurityDbContext: IdentityDbContext
    {
        public AppSecurityDbContext(DbContextOptions<AppSecurityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
