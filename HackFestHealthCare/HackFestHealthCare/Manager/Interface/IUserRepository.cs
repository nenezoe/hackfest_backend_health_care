using HackFestHealthCare.Models;
using HackFestHealthCare.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Interface
{
    public interface IUserRepository
    {
        Task<UserModel> CreateUser(UserModel model);
        Task<UserModel> DeleteUser(UserModel model);
        Task<string> GetPasswordHash(Guid userId);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel> GetUserById(Guid userId);
        Task<string> GetProfileImageUri(Guid userId);
        Task<UserModel[]> GetUsers();
        Task<List<UserModel>> GetAllUsers();
        Task<UserModel[]> GetUsers(int offset, int limit);
        Task SetPasswordHash(Guid userId, string passwordHash);
        Task<UserModel> UpdateUser(UserModel model);
        Task<UserModel[]> FindUser(string search);
        Task<UserModel[]> GetUsersInRole(string rolename);
        Task<UserModel> DisableUser(Guid userId);
        Task<int> SetEmailConfirmed(Guid userId);
        Task<bool> SaveAll();
        Task TokenCreateModel(Guid userId, TokenModel token);
        Task<int> CreateRefreshTokenModel(string clientId, string refreshToken, TokenModel model);
    }
}
