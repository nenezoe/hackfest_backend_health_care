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
    public class UserRepository : IUserRepository
    {
        private DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> CreateUser(UserModel model)
        {
            var user = new UserProfile
            {
                Id = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                IsAccountActivated = true,
                AccountActivationDate = null,
                Address = model.Address,
                CreatedAt = DateTime.Now,
                CreatedBy = model.UserId
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            model.UserId = user.Id;
            return model;
        }

        public async Task<UserModel> DeleteUser(UserModel model)
        {
            var user = await _context.Set<UserProfile>().FindAsync(model.UserId);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<string> GetPasswordHash(Guid userId)
        {
            var user = await _context.Set<UserProfile>().FindAsync(userId);
            return user.PasswordHash;
        }

        public async Task<UserModel> GetUserById(Guid userId)
        {
            var user = await (from x in _context.Set<UserProfile>()
                              where x.IsDeleted == false && x.Id == userId
                              select new UserModel
                              {
                                  UserId = x.Id,
                                  FirstName = x.FirstName,
                                  LastName = x.LastName,
                                  Email = x.Email,
                                  PhoneNumber = x.PhoneNumber,
                                  Address = x.Address,
                                  IsAccountActivated = x.IsAccountActivated,
                                  CreatedAt = x.CreatedAt,
                              }).FirstOrDefaultAsync();
            return user;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var query = from x in _context.Set<UserProfile>()
                        where x.IsDeleted == false && x.Email.ToLower() == email
                        select new UserModel
                        {
                            UserId = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber,
                            Address = x.Address,
                            IsAccountActivated = x.IsAccountActivated,
                            CreatedAt = x.CreatedAt
                        };

            var users = await query.FirstOrDefaultAsync();
            return users;
        }

        public async Task<UserModel[]> FindUser(string search)
        {
            var query = from x in _context.Set<UserProfile>()
                        where x.Email.Contains(search)
                        || x.FirstName.Contains(search)
                        || x.LastName.Contains(search)
                        || x.PhoneNumber.Contains(search)
                        select new UserModel
                        {
                            UserId = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber,
                            Address = x.Address,
                            IsAccountActivated = x.IsAccountActivated,
                            CreatedAt = x.CreatedAt
                        };

            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            var users = await (from x in _context.Set<UserProfile>()
                               join userrole in _context.Set<UserRole>() on x.Id equals userrole.UserId
                               join role in _context.Set<Role>() on userrole.RoleId equals role.RoleId
                               where x.IsDeleted == false && (role.RoleName == "user")
                               orderby x.CreatedAt descending
                               select new UserModel
                               {
                                   UserId = x.Id,
                                   FirstName = x.FirstName,
                                   LastName = x.LastName,
                                   Email = x.Email,
                                   PhoneNumber = x.PhoneNumber,
                                   Address = x.Address,
                                   IsAccountActivated = x.IsAccountActivated,
                                   CreatedAt = x.CreatedAt
                               }).ToListAsync();
            return users;
        }
        public async Task<string> GetProfileImageUri(Guid userId)
        {
            var user = await _context.Set<UserProfile>().FindAsync(userId);
            return user.PicturePath;
        }

        public async Task<UserModel[]> GetUsers()
        {
            var query = from x in _context.Set<UserProfile>()
                        where x.IsDeleted == false
                        select new UserModel
                        {
                            UserId = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber,
                            Address = x.Address,
                            IsAccountActivated = x.IsAccountActivated,
                            CreatedAt = x.CreatedAt
                        };
            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<UserModel[]> GetUsers(int offset, int limit)
        {
            var query = from user in _context.Set<UserProfile>()
                        select new UserModel
                        {
                            UserId = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Address = user.Address,
                            IsAccountActivated = user.IsAccountActivated,
                            CreatedAt = user.CreatedAt
                        };

            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<UserModel[]> GetUsersInRole(string rolename)
        {
            var query = from user in _context.Set<UserProfile>()
                        join userrole in _context.Set<UserRole>() on user.Id equals userrole.UserId
                        join role in _context.Set<Role>() on userrole.RoleId equals role.RoleId
                        where role.RoleName == rolename
                        select new UserModel
                        {
                            UserId = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Address = user.Address,
                            IsAccountActivated = user.IsAccountActivated,
                            CreatedAt = user.CreatedAt,
                            Roles = user.UserRoles.Select(x => new RoleModel { RoleId = x.RoleId, Name = x.Role.RoleName }).ToArray()
                        };
            var returnedUser = await query.ToArrayAsync();
            return returnedUser;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> SetEmailConfirmed(Guid userId)
        {
            var user = await _context.Set<UserProfile>().FindAsync(userId);
            if (user == null) return -1;
            if (user.IsAccountActivated == true) return -2;

            user.IsAccountActivated = true;
            user.AccountActivationDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return 1;
        }

        public async Task SetPasswordHash(Guid userId, string passwordHash)
        {
            var user = await _context.Set<UserProfile>().FindAsync(userId);
            user.PasswordHash = passwordHash;
            await _context.SaveChangesAsync();
        }
        public async Task<UserModel> DisableUser(Guid userId)
        {
            var query = await (from user in _context.Set<UserProfile>()
                               where user.Id == userId
                               select user).FirstOrDefaultAsync();
            query.IsDeleted = !query.IsDeleted;
            await _context.SaveChangesAsync();
            return new UserModel
            {
                FirstName = query.FirstName,
                LastName = query.LastName,
                Email = query.Email,
                PhoneNumber = query.PhoneNumber,
                IsAccountActivated = query.IsAccountActivated,
                CreatedAt = query.CreatedAt,
                UserId = query.Id
            };
        }

        public async Task<UserModel> UpdateUser(UserModel model)
        {
            var user = await _context.Set<UserProfile>().FindAsync(model.UserId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = user.Email;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            await _context.SaveChangesAsync();
            return model;
        }


        public async Task TokenCreateModel(Guid userId, TokenModel token)
        {
            // first we delete any existing old refreshtokens
            var oldrTokens = _context.Set<TokenModel>().Where(rt => rt.UserId == userId);

            if (oldrTokens != null)
            {
                foreach (var oldrt in oldrTokens)
                {
                    _context.Set<TokenModel>().Remove(oldrt);
                }
            }
            // Add new refresh token to Database
            _context.Set<TokenModel>().Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateRefreshTokenModel(string clientId, string refreshToken, TokenModel model)
        {
            // check if the received refreshToken exists for the given clientId
            var rt = await _context.Set<TokenModel>().Where(r => r.ClientId == clientId && r.Value == refreshToken).FirstOrDefaultAsync();
            if (rt == null)
            {
                // refresh token not found or invalid (or invalid clientId)
                return -1;
            }
            // check if refresh token is expired
            if (rt.ExpiryTime < DateTime.UtcNow)
            {
                return -2;
            }
            _context.Set<TokenModel>().Remove(rt);

            // Add new refresh token to Database
            _context.Set<TokenModel>().Add(model);
            await _context.SaveChangesAsync();

            return 1;
        }
    }
}
