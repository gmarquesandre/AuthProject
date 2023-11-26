using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AuthProject.Startup
{
    public class ProviderConfiguration(string connString)
    {
        private readonly string _connectionString = connString;
        public ProviderConfiguration With() => this;
        private static readonly string MigrationAssembly = typeof(ProviderConfiguration)!.GetTypeInfo()!.Assembly!.GetName()!.Name!;

        public static ProviderConfiguration Build(string connString)
        {
            return new ProviderConfiguration(connString);
        }

        public Action<DbContextOptionsBuilder> SqlServer =>
            options => options.UseSqlServer(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));

        public Action<DbContextOptionsBuilder> MySql =>
            options => options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString), sql => sql.MigrationsAssembly(MigrationAssembly));       

        public Action<DbContextOptionsBuilder> Postgre =>
            options => 
            {
                options.UseNpgsql(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            };

        public Action<DbContextOptionsBuilder> Sqlite =>
                options => options.UseSqlite(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));

        /// <summary>
        /// it's just a tuple. Returns 2 parameters.
        /// Trying to improve readability at ConfigureServices
        /// </summary>
        public static (DatabaseType, string) DetectDatabase(IConfiguration configuration) => (
            configuration.GetValue("Appsettings:DatabaseType", DatabaseType.None),
            configuration.GetConnectionString("DefaultConnection") ?? string.Empty);
    }


}
