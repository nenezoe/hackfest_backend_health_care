using HackFestHealthCare.Extensions;
using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<UserModel> _userManager;

        public UserController(IUserRepository userRepo, UserManager<UserModel> userManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdministratorRole")]
        [SwaggerOperation(Summary = "Description:Try to register a user account depending on roles")]
        [HttpPost("add_user")]
        public async Task<IActionResult> RegisterUser([FromBody] AddUserVModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var email = model.Email.ToLower();
                    if (model.Password != model.RTPassword) return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("Password & Repeat password does not match."));
                    if (model.Password.ToString().Trim().Length < 6) return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("Password length must be greater that 6 characters."));
                    var userCheck = await _userRepo.GetUserByEmail(email);
                    if (userCheck != null) return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("User with this email address already exist, kindly use another email address"));

                    var user = new UserModel
                    {
                        UserId = Guid.NewGuid(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
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
                    createResult = await _userManager.AddToRolesAsync(user, model.Roles);
                    if (!createResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        throw new Exception(createResult.GetErrors());
                    }
                    return Ok(new { code = 200, message = "Registration successful", succeeded = true });
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured(ex.Message));
            }
        }

        [Authorize(Policy = "RequireAdministratorRole")]
        [SwaggerOperation(Summary = "Description:Returns a list of Users")]
        [HttpGet("get_users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var user = await _userRepo.GetAllUsers();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured(ex.Message));
            }
        }

        [SwaggerOperation(Summary = "Description:Gets a specific activated user details by id")]
        [HttpGet("get_user_by_id/{id:guid}")]
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userRepo.GetUserById(id);
                if (user != null) return Ok(user);
                return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("Invalid User Id"));
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured(ex.Message));
            }
        }

        [SwaggerOperation(Summary = "Description:Update user account")]
        [HttpPost("update_user/{id:guid}")]
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserProfileViewModel model)
        {
            try
            {
                var user = await _userRepo.GetUserById(id);
                if (user == null) return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured("Invalid User Id"));
                var updateDto = new UserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email.ToLower(),
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                var response = await _userRepo.UpdateUser(updateDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<UserProfileViewModel>.ErrorOccured(ex.Message));
            }
        }

        [SwaggerOperation(Summary = "Description:Search for user by email, firstname, lastname, phone number")]
        [HttpPost("search_user")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> SearchUser(string search)
        {
            try
            {
                if (string.IsNullOrEmpty(search)) return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("Search cannot be empty"));
                var user = await _userRepo.FindUser(search);
                if (user.Any()) return Ok(user);
                return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<UserProfileViewModel>.ErrorOccured(ex.Message));
            }
        }

        [SwaggerOperation(Summary = "Description:Change user password")]
        [HttpPost("change_password/{id:guid}")]
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.NewPassword != model.ConfirmNewPassword) return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured("Password & Repeat password does not match."));
                    var user = await _userRepo.GetUserById(id);
                    if (user == null) return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured("Invalid User Id"));

                    if (await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return Ok(user);
                        }
                        return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured("Failed"));
                    }
                    return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured("Your current password is in-correct"));
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<ChangePasswordModel>.ErrorOccured(ex.Message));
            }
        }

        [SwaggerOperation(Summary = "Description:Delete user Account")]
        [HttpPost("delete_user/{id:guid}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _userRepo.GetUserById(id);
                if (user == null) return BadRequest(WebApiResponses<AddUserVModel>.ErrorOccured("Invalid User Id")); 

                //delete user in role
                var rolesForUser = await _userManager.GetRolesAsync(user);
                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, item);
                    }
                }
                //delete user
                var deleteUser = await _userManager.DeleteAsync(user);
                if (!deleteUser.Succeeded) return BadRequest(WebApiResponses<UserProfileViewModel>.ErrorOccured("An error occur while deleting user"));
                return Ok("User Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(WebApiResponses<UserProfileViewModel>.ErrorOccured(ex.Message));
            }
        }
    }
}
