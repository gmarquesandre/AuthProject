using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace AuthProject.Startup
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext(options), ISecurityKeyContext
    {
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}

