using HackFestHealthCare.ViewModel;
using System;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Interface
{
    public interface IRoleRepository
    {
        Task<RoleModel> CreateRole(RoleModel role);
        Task DeleteRole(RoleModel role);
        Task<RoleModel> GetRoleById(string roleId);
        Task<RoleModel> GetRoleByName(string roleName);
        Task UpdateRole(RoleModel role);
        Task<RoleModel[]> GetRoles();
        RoleModel[] GetRolesQuerable();
        Task AddUserToRole(Guid userId, string roleId);
        Task<UserModel> RemoveUserFromRole(Guid userId, string roleId);
        Task<RoleModel[]> GetUserRoles(Guid userId);
        Task<bool> IsUserInRole(UserModel user, string roleName);

        Task<RoleModel[]> ByUserId(int clientId, Guid userId);

        Task<UserRoleModel> GetUserRole(Guid userId, string roleId);
    }
}
