using AuthProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Identity.Interfaces;
using NetDevPack.Security.Jwt.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static AuthProject.Models.AuthController;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace AuthProject.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public partial class AuthController(UserManager<IdentityUser> userManager, IJwtBuilder jwtBuilder, SignInManager<IdentityUser> signInManager, IJwtService jwtService) : MainController
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IJwtBuilder _jwtBuilder = jwtBuilder;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IJwtService _jwtService = jwtService;

        [HttpPost("new-account")]
        public async Task<IActionResult> Register(CreateUser newUser)
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
        [HttpPost("auth")]
        public async Task<ActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password,
                false, true);

            if (result.Succeeded)
            {


                var signingCredentials = await _jwtService.GetCurrentSigningCredentials();



                //var token = new JwtSecurityToken(
                //            issuer: "teste",
                //            audience: "qualquer",
                //            claims: new List<Claim>()
                //            {
                //                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //                new(JwtRegisteredClaimNames.Sub, result.)
                //            }
                //        );

                //string jwt = new JwtSecurityTokenHandler().WriteToken(token);


                var jwt = await _jwtBuilder
                                            .WithEmail(userLogin.Email)
                                            .WithJwtClaims()
                                            .WithUserClaims()
                                            .WithUserRoles()
                                            .WithRefreshToken()
                                            .BuildUserResponse();
                return CustomResponse(jwt);
            }

            if (result.IsLockedOut)
            {
                AddErrorToStack("User temporary blocked. Too many tries.");
                return CustomResponse();
            }

            AddErrorToStack("User or Password incorrect");
            return CustomResponse();
        }
        [HttpGet("authenticated-request")]
        [Authorize]
        public IActionResult ValidateTokenAsync()
        {
            return Ok("Oi");
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(RequestRefreshToken request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                AddErrorToStack("Invalid Refresh Token");
                return CustomResponse();
            }

            var token = await _jwtBuilder.ValidateRefreshToken(request.RefreshToken);

            if (!token.IsValid)
            {
                AddErrorToStack("Expired Refresh Token");
                return CustomResponse();
            }

            var jwt = await _jwtBuilder
                                        .WithUserId(token.UserId)
                                        .WithJwtClaims()
                                        .WithUserClaims()
                                        .WithUserRoles()
                                        .WithRefreshToken()
                                        .BuildUserResponse();

            return CustomResponse(jwt);
        }

    }
}
