using Microsoft.AspNetCore.Mvc.Testing;

namespace AuthProject.IntegrationTests.Infraestructure
{
    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture> { }
    public partial class IntegrationTestsFixture : IDisposable
    {
        public ApiFactory<Program> Factory;
        public HttpClient Client;

        private static readonly WebApplicationFactoryClientOptions ClientOptions = new()
        {
            AllowAutoRedirect = true,
            BaseAddress = new Uri("http://localhost"),
            HandleCookies = true,
            MaxAutomaticRedirections = 7
        };
        public IntegrationTestsFixture()
        {
            Factory = new ApiFactory<Program>();
            Client = Factory.CreateClient(ClientOptions);
        }

        void IDisposable.Dispose()
        {
            Client?.Dispose();
            Factory?.Dispose();
            GC.SuppressFinalize(this);

        }
    }
}