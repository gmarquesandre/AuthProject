using AuthProject.IntegrationTests.Infraestructure;
using AuthProject.Models;
using Bogus;
using NetDevPack.Identity.Jwt.Model;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using static AuthProject.Models.AuthController;

namespace AuthProject.IntegrationTests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class TestOperations
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly Faker _faker = new();
        public TestOperations(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }


        [Trait("Auth", "Api Testing")]
        [Fact(DisplayName ="Teste Operações")]
        public async Task Test()
        {
            string password = _faker.Random.AlphaNumeric(8);
            var newUser = new CreateUser() { Email = _faker.Person.Email, Password = password, RePassword = password };
            //Must Create User Successfully
            var responseCreate = await _fixture.Client.PostAsJsonAsync("/api/identity/new-account", newUser);

            Assert.Equal(HttpStatusCode.OK, responseCreate.StatusCode);


            var responseAuthenticate = await _fixture.Client.PostAsJsonAsync("/api/identity/auth", new UserLogin() { Email = newUser.Email, Password = newUser.Password });
            var authenticateObject = JsonConvert.DeserializeObject<UserResponse>(await responseAuthenticate.Content.ReadAsStringAsync())!;
            Assert.Equal(HttpStatusCode.OK, responseAuthenticate.StatusCode);

           
            var responseRefreshToken = await _fixture.Client.PostAsJsonAsync("/api/identity/refresh-token", new RequestRefreshToken() { RefreshToken = authenticateObject.RefreshToken});
            string responseRefreshTokenString = await responseRefreshToken.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, responseRefreshToken.StatusCode);

        }
    }
}