using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.Models;
using HackFestHealthCare.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private DbContext _context;

        public RoleRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddUserToRole(Guid userId, string roleId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.Set<UserRole>().Add(userRole);
            await _context.SaveChangesAsync();
        }

        public Task<RoleModel[]> ByUserId(int clientId, Guid userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RoleModel> CreateRole(RoleModel model)
        {
            var check = await _context.Set<Role>().Where(x => x.RoleName == model.Name).FirstOrDefaultAsync();
            if (check != null) throw new Exception("Role already exist");
            var role = new Role
            {
                RoleName = model.Name,
            };
            role.RoleId = Guid.NewGuid().ToString();
            _context.Set<Role>().Add(role);
            await _context.SaveChangesAsync();
            model.RoleId = role.RoleId;
            return model;
        }

        public async Task DeleteRole(RoleModel model)
        {
            var role = await _context.Set<Role>().FindAsync(model.RoleId);
            _context.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<RoleModel> GetRoleById(string roleId)
        {
            var query = _context.Set<Role>().Where(x => x.RoleId == roleId);

            var role = await query.FirstOrDefaultAsync();
            if (role == null)
            {
                return null;
            }
            return new RoleModel
            {
                Name = role.RoleName,
                RoleId = role.RoleId
            };
        }

        public async Task<RoleModel> GetRoleByName(string roleName)
        {
            var query = _context.Set<Role>().Where(x => x.RoleName == roleName);

            var role = await query.FirstOrDefaultAsync();
            if (role == null)
            {
                return null;
            }
            return new RoleModel
            {
                Name = role.RoleName,
                RoleId = role.RoleId
            };
        }

        public async Task<RoleModel[]> GetRoles()
        {
            var result = await _context.Set<Role>().Where(x => x.RoleName != "Power_User").ToArrayAsync();
            return result.Select(role => new RoleModel
            {
                Name = role.RoleName,
                RoleId = role.RoleId
            }).ToArray();
        }

        public RoleModel[] GetRolesQuerable()
        {
            var result = _context.Set<Role>().ToArray();
            return result.Select(role => new RoleModel
            {
                Name = role.RoleName,
                RoleId = role.RoleId
            }).ToArray();
        }

        public async Task<UserRoleModel> GetUserRole(Guid userId, string roleId)
        {
            var urole = await _context.Set<UserRole>().FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            return new UserRoleModel
            {
                UserRoleId = urole.Id,
                RoleId = urole.RoleId,
                UserId = urole.UserId
            };
        }

        public async Task<RoleModel[]> GetUserRoles(Guid userId)
        {
            var query = _context.Set<UserRole>().Include(x => x.Role).Where(r => r.UserId == userId);

            var result = await query.Select(x => x.Role).ToArrayAsync();
            return result.Select(role => new RoleModel
            {
                Name = role.RoleName,
                RoleId = role.RoleId
            }).ToArray();
        }

        public async Task<bool> IsUserInRole(UserModel user, string roleName)
        {
            var result = await _context.Set<UserRole>().Include(d => d.Role).FirstOrDefaultAsync(x => x.UserId == user.UserId && x.Role.RoleName == roleName);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<UserModel> RemoveUserFromRole(Guid userId, string roleId)
        {
            var model = await _context.Set<UserRole>().Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            if (model == null)
            {
                throw new Exception("User does not exist in role");
            }

            _context.Set<UserRole>().Remove(model);
            await _context.SaveChangesAsync();
            var user = model.User;
            return new UserModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsAccountActivated = user.IsAccountActivated,
                UserId = user.Id
            };

        }

        public async Task UpdateRole(RoleModel role)
        {
            var resp = await _context.Set<Role>().FirstOrDefaultAsync(r => r.RoleId == role.RoleId);
            resp.RoleName = role.Name;
            await _context.SaveChangesAsync();
        }
    }

}
