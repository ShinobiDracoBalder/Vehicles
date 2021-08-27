using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Vehicles.API.Data.Entities;

namespace Vehicles.API.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<User> GetUserAsync(Guid id);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);


    }
}
