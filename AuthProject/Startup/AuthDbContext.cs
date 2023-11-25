using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace AuthProject.Startup
{
    public class AuthDbContext : IdentityDbContext, ISecurityKeyContext
    {        
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            
        }
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            IdentityUser admin = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                NormalizedUserName = "ADMIN",
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                ConcurrencyStamp = null,
                LockoutEnd = null,
                PasswordHash = null,
                PhoneNumber = null,
                AccessFailedCount = 0
                
            };
            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "senhaAdmin");
           
            builder.Entity<IdentityUser>().HasData(admin);
        }

    }
}
