using AuthProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.PasswordHasher.Core;
using System.Reflection;

namespace AuthProject.Startup
{
    public static class IdentityConfig
    {
        public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DB_AUTH")!;

           services.AddMemoryCache()
                .AddDataProtection();

            services.AddJwtConfiguration(configuration, "AppSettings")
                .AddNetDevPackIdentity<IdentityUser>()
                .PersistKeysToDatabaseStore<AuthDbContext>();

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

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

    public static class ProviderSelector
    {
        public static IServiceCollection ConfigureProviderForContext<TContext>(
            this IServiceCollection services,
            (DatabaseType, string) options) where TContext : DbContext
        {
            var (database, connString) = options;
            return services.PersistStore<TContext>(ProviderConfiguration.Build(connString).With().SqlServer);
            //return database switch
            //{
            //    DatabaseType.SqlServer => services.PersistStore<TContext>(Build(connString).With().SqlServer),
            //    DatabaseType.MySql => services.PersistStore<TContext>(Build(connString).With().MySql),
            //    DatabaseType.Postgre => services.PersistStore<TContext>(Build(connString).With().Postgre),
            //    DatabaseType.Sqlite => services.PersistStore<TContext>(Build(connString).With().Sqlite),

            //    _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            //};
        }
        public static IServiceCollection PersistStore<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> databaseConfig) where TContext : DbContext
        {
            // Add a DbContext to store Keys. SigningCredentials and DataProtectionKeys
            if (services.All(x => x.ServiceType != typeof(TContext)))
                services.AddDbContext<TContext>(databaseConfig);
            return services;
        }

        public static Action<DbContextOptionsBuilder> WithProviderAutoSelection((DatabaseType, string) options)
        {
            var (database, connString) = options;

            return ProviderConfiguration.Build(connString).With().SqlServer;   
            //return database switch
            //{
            //    DatabaseType.SqlServer => Build(connString).With().SqlServer,
            //    DatabaseType.MySql => Build(connString).With().MySql,
            //    DatabaseType.Postgre => Build(connString).With().Postgre,
            //    DatabaseType.Sqlite => Build(connString).With().Sqlite,

            //    _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            //};
        }

    }
    public enum DatabaseType
    {
        None,
        SqlServer,
        MySql,
        Postgre,
        Sqlite,
    }
    public class ProviderConfiguration
    {
        private readonly string _connectionString;
        public ProviderConfiguration With() => this;
        private static readonly string MigrationAssembly = typeof(ProviderConfiguration).GetTypeInfo().Assembly.GetName().Name;

        public static ProviderConfiguration Build(string connString)
        {
            return new ProviderConfiguration(connString);
        }



        public ProviderConfiguration(string connString)
        {
            _connectionString = connString;
        }

        public Action<DbContextOptionsBuilder> SqlServer =>
            options => options.UseSqlServer(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));

        public Action<DbContextOptionsBuilder> MySql =>
            options => options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString), sql => sql.MigrationsAssembly(MigrationAssembly));

        public Action<DbContextOptionsBuilder> Postgre =>
            options => throw new NotImplementedException();
            //{
            //    options.UseNpgsql(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));
            //    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //};

        public Action<DbContextOptionsBuilder> Sqlite =>
            options => throw new NotImplementedException();  //options.UseSqlite(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));


        /// <summary>
        /// it's just a tuple. Returns 2 parameters.
        /// Trying to improve readability at ConfigureServices
        /// </summary>
        public static (DatabaseType, string) DetectDatabase(IConfiguration configuration) => (
            configuration.GetValue<DatabaseType>("AppSettings:DatabaseType", DatabaseType.None),
            configuration.GetConnectionString("DefaultConnection"));
    }
}
