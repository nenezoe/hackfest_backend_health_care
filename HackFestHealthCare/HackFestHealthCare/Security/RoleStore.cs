using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.ViewModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HackFestHealthCare.Security
{
    public class RoleStore : IRoleStore<RoleModel>
    {
        private readonly IRoleRepository _role;

        public RoleStore(IRoleRepository role)
        {
            _role = role;
        }

        public async Task<IdentityResult> CreateAsync(RoleModel role, CancellationToken cancellationToken)
        {
            try
            {
                await _role.CreateRole(role);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                var error = new IdentityError { Code = "IDError04", Description = ex.Message };
                return IdentityResult.Failed(error);
            }
        }

        public async Task<IdentityResult> DeleteAsync(RoleModel role, CancellationToken cancellationToken)
        {
            try
            {
                await _role.DeleteRole(role);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                var error = new IdentityError { Code = "IDError05", Description = ex.Message };
                return IdentityResult.Failed(error);
            }
        }

        public void Dispose()
        {
            // Do Nothing
        }

        public Task<RoleModel> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return _role.GetRoleById(roleId);
        }

        public Task<RoleModel> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _role.GetRoleByName(normalizedRoleName);
        }

        public Task<string> GetNormalizedRoleNameAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name.ToLower());
        }

        public Task<string> GetRoleIdAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.RoleId.ToString().ToLower());
        }

        public Task<string> GetRoleNameAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name.ToLower());
        }

        public Task SetNormalizedRoleNameAsync(RoleModel role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetRoleNameAsync(RoleModel role, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public async Task<IdentityResult> UpdateAsync(RoleModel role, CancellationToken cancellationToken)
        {
            try
            {
                await _role.UpdateRole(role);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                var error = new IdentityError { Code = "IDError06", Description = ex.Message };
                return IdentityResult.Failed(error);
            }
        }
    }
}
