using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.Jwt.Core;
using NetDevPack.Security.PasswordHasher.Core;
using System.Diagnostics;

namespace AuthProject.Startup
{
    public static class IdentityConfig
    {
        public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureProviderForContext<AuthDbContext>(ProviderConfiguration.DetectDatabase(configuration));
            services.AddMemoryCache()
                .AddDataProtection();



            services.AddJwtConfiguration(configuration, "AppSettings")
                .AddNetDevPackIdentity<IdentityUser>(opt => {
                    opt.CacheTime = TimeSpan.FromMinutes(5);
                    opt.KeyPrefix = "MTEST";
                })
                .PersistKeysToDatabaseStore<AuthDbContext>();

          
            services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();


            services.UpgradePasswordSecurity()
               .WithStrengthen(PasswordHasherStrength.Moderate)
               .UseArgon2<IdentityUser>();


        }
    }
}
