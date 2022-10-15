using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace HackFestHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdministratorRole")]
    public class RolesController : BaseController
    {
        private readonly IRoleRepository _roleRepo;
        public RolesController(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        [SwaggerOperation(Summary = "Description: Adds a new Role")]
        [HttpPost("add_role")]
        [ProducesDefaultResponseType(typeof(APIResponseModel<RoleModel>))]
        public async Task<IActionResult> AddRole([FromBody] RoleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _roleRepo.CreateRole(model));
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<RoleModel>.ErrorOccured(ex.Message));
            }
        }
        [SwaggerOperation(Summary = "Description:Returns a list of Roles")]
        [HttpGet("Get_roles")]
        [ProducesDefaultResponseType(typeof(APIResponseModel<RoleModel>))]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                return Ok(await _roleRepo.GetRoles());
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<RoleModel>.ErrorOccured(ex.Message));
            }
        }
        [SwaggerOperation(Summary = "Description:Returns a single Role entry")]
        [HttpGet("get_role/{Id}")]
        [ProducesDefaultResponseType(typeof(APIResponseModel<RoleModel>))]
        public async Task<IActionResult> GetRole(string Id)
        {
            try
            {
                return Ok(await _roleRepo.GetRoleById(Id));
            }
            catch (Exception ex)
            {
                return Ok(WebApiResponses<RoleModel>.ErrorOccured(ex.Message));
            }
        }
    }
}
