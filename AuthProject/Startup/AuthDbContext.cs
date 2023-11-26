using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;
using NetDevPack.Security.PasswordHasher.Core;
using NetDevPack.Security.PasswordHasher.Core.DependencyInjection;

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
        }

    }
}

