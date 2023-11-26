using Microsoft.EntityFrameworkCore;

namespace AuthProject.Startup
{
    public static class ProviderSelector
    {
        public static IServiceCollection ConfigureProviderForContext<TContext>(
            this IServiceCollection services,
            (DatabaseType, string) options) where TContext : DbContext
        {
            var (database, connString) = options;

            return database switch
            {
                DatabaseType.SqlServer => services.PersistStore<TContext>(ProviderConfiguration.Build(connString).With().SqlServer),
                DatabaseType.MySql => services.PersistStore<TContext>(ProviderConfiguration.Build(connString).With().MySql),
                DatabaseType.Postgre => services.PersistStore<TContext>(ProviderConfiguration.Build(connString).With().Postgre),
                DatabaseType.Sqlite => services.PersistStore<TContext>(ProviderConfiguration.Build(connString).With().Sqlite),

                _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            };
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

            return database switch
            {
                DatabaseType.SqlServer => ProviderConfiguration.Build(connString).With().SqlServer,
                DatabaseType.MySql => ProviderConfiguration.Build(connString).With().MySql,
                DatabaseType.Postgre => ProviderConfiguration.Build(connString).With().Postgre,
                DatabaseType.Sqlite => ProviderConfiguration.Build(connString).With().Sqlite,

                _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            };
        }

    }
}
