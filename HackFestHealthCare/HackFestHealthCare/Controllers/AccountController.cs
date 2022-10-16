using HackFestHealthCare.Extensions;
using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.Models;
using HackFestHealthCare.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HackFestHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IUserRepository _userRepo;

        public AccountController(
            UserManager<UserModel> userManager,
            IUserRepository userRepo)
        {
            _userManager = userManager;
            _userRepo = userRepo;
        }

        [SwaggerOperation(Summary = "Description:Try to register a user account")]
        [HttpPost("register")]
        [ProducesDefaultResponseType(typeof(APIResponseModel<RegisterVModel>))]
        public async Task<IActionResult> Register([FromBody] RegisterVModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var email = model.Email.ToLower();
                    if (model.Password != model.RTPassword) return BadRequest(WebApiResponses<RegisterVModel>.ErrorOccured("Password & Repeat password does not match."));
                    if (model.Password.ToString().Trim().Length < 6) return BadRequest(WebApiResponses<RegisterVModel>.ErrorOccured("Password length must be greater that 6 characters."));
                    var userCheck = await _userRepo.GetUserByEmail(email);
                    if (userCheck != null) return BadRequest(WebApiResponses<RegisterVModel>.ErrorOccured("User with this email address already exist, kindly use another email address"));
                   
                    var user = new UserModel
                    {
                        UserId = Guid.NewGuid(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email.ToLower(),
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address
                    };
                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        throw new Exception(createResult.GetErrors());
                    }
                    var addPassword = await _userManager.AddPasswordAsync(user, model.Password);
                    if (!addPassword.Succeeded)
                    {
                        //Delete User and Throw Exception
                        await _userManager.DeleteAsync(user);
                        throw new Exception(addPassword.GetErrors());
                    }
                    createResult = await _userManager.AddToRoleAsync(user, "user");
                    if (!createResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        throw new Exception(createResult.GetErrors());
                    }
                    return Ok(DxResponse.Success(user));
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<RegisterVModel>.ErrorOccured(ex.Message));
            }
        }

        private string[] GenerateJwtToken(UserModel user, string fullname, string[] roles)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var _config = builder.Build();

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"]));
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiry"]));
            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity getClaimsIdentity()
            {
                return new ClaimsIdentity(
                    getClaims()
                );

                Claim[] getClaims()
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                    claims.Add(new Claim(ClaimTypes.Name, fullname));
                    claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
                    claims.Add(new Claim("LoggedOn", DateTime.Now.ToString()));

                    foreach (var item in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }
                    return claims.ToArray();
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = getClaimsIdentity(),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Issuer"],
                NotBefore = DateTime.Now,
                Expires = expires
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            var expiration = expires;
            string[] result = { jwt, expiration.ToString() };
            return result;
        }

        [SwaggerOperation(Summary = "Description:Authenticate a user with token/refresh token.")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model)
        {
            if (model == null)
            {
                return new StatusCodeResult(500);
            }

            switch (model.GrantType)
            {
                case "password":
                    return await GenerateNewToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    // not supported - return a HTTP 401 (Unauthorized)
                    return new UnauthorizedResult();
            }

        }

        // Method to Create New JWT and Refresh Token
        private async Task<IActionResult> GenerateNewToken(TokenRequestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email.ToLower());
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var fullname = user.LastName + " " + user.FirstName;
                        
                        var roles = await _userManager.GetRolesAsync(user);

                        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                        var _config = builder.Build();
                        // username & password matches: create the refresh token
                        var newRtoken = CreateRefreshToken(_config["Jwt:ClientId"], user.UserId);
                        // first we delete any existing old refreshtokens
                        await _userRepo.TokenCreateModel(user.UserId, newRtoken);
                        // Create & Return the access token which contains JWT and Refresh Token
                        var accessToken = await CreateAccessToken(user, newRtoken.Value, fullname, roles.ToArray());

                        return Ok(new
                        {
                            access_token = accessToken,
                            succeeded = true
                        });
                    }
                    return BadRequest(new { message = "Please Check the Login Credentials - Invalid Email/Password was entered!", succeeded = false });
                }
                return Ok(WebApiResponses<TokenRequestModel>.ErrorOccured($"{ModelState}"));
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<TokenRequestModel>.ErrorOccured(ex.Message));
            }
        }

        private Task<TokenResponseModel> CreateAccessToken(UserModel user, string refreshToken, string fullname, string[] roles)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var _config = builder.Build();

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"]));
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiry"]));
            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity getClaimsIdentity()
            {
                return new ClaimsIdentity(
                    getClaims()
                );

                Claim[] getClaims()
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                    claims.Add(new Claim(ClaimTypes.Name, fullname));
                    claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
                    claims.Add(new Claim("LoggedOn", DateTime.Now.ToString()));

                    foreach (var item in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }
                    return claims.ToArray();
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = getClaimsIdentity(),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Issuer"],
                NotBefore = DateTime.Now,
                Expires = expires
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return Task.FromResult(new TokenResponseModel()
            {
                token = jwt,
                userId = user.UserId,
                expiration = expires,
                refresh_token = refreshToken,
                roles = roles.ToArray()
            });
        }
        private TokenModel CreateRefreshToken(string clientId, Guid userId)
        {
            return new TokenModel()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(90)
            };
        }

        private async Task<IActionResult> RefreshToken(TokenRequestModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email.ToLower());
                if (user == null)
                {
                    return BadRequest(new { message = "Please Check the Login Credentials - Invalid Email/Password was entered!", succeeded = false });
                }
                var fullname = user.LastName + " " + user.FirstName;
                var roles = await _userManager.GetRolesAsync(user);
                // generate a new refresh token
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var _config = builder.Build();
                var newRtoken = CreateRefreshToken(_config["Jwt:ClientId"], user.UserId);
                // clientId, userId,
                var rtNew = await _userRepo.CreateRefreshTokenModel(_config["Jwt:ClientId"], model.RefreshToken, newRtoken);
                if (rtNew == -1 || rtNew == -2)
                {
                    return new UnauthorizedResult();
                }
                else
                {
                    var response = await CreateAccessToken(user, newRtoken.Value, fullname, roles.ToArray());
                    return Ok(new { access_token = response });
                }
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<TokenRequestModel>.ErrorOccured(ex.Message));
            }
        }
    }
}
