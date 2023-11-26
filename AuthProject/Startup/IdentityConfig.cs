using Microsoft.AspNetCore.Identity;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.PasswordHasher.Core;

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
                    opt.DaysUntilExpire = 0;                    
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
