using AuthProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.Identity.Interfaces;

namespace AuthProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : MainController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtBuilder _jwtBuilder;
        public AuthController(UserManager<IdentityUser> userManager, IJwtBuilder jwtBuilder)
        {
            _userManager = userManager;
            _jwtBuilder = jwtBuilder;
        }

        [HttpPost("new-account")]
        public async Task<IActionResult> Register(CreateUserDto newUser)
        {
            var user = new IdentityUser
            {
                UserName = newUser.Email,
                Email = newUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, newUser.Password);

            if (result.Succeeded)
            {
                //var customerResult = await RegisterUser(newUser);

                //if (!customerResult.ValidationResult.IsValid)
                //{
                //    await _userManager.DeleteAsync(user);
                //    return CustomResponse(customerResult.ValidationResult);
                //}

                var jwt = await _jwtBuilder
                                            .WithEmail(newUser.Email)
                                            .WithJwtClaims()
                                            .WithUserClaims()
                                            .WithUserRoles()
                                            .WithRefreshToken()
                                            .BuildUserResponse();

                return CustomResponse(jwt);
            }

            foreach (var error in result.Errors)
            {
                AddErrorToStack(error.Description);
            }

            return CustomResponse();
        }

    }
}
