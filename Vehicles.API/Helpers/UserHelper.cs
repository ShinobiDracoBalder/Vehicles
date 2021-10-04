using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;
using Vehicles.API.Models;
using Vehicles.Common.Enums;

namespace Vehicles.API.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            DataContext dataContext, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }
        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }
        }
        public async Task<User> GetUserAsync(string email)
        {
            return await _dataContext.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehiclePhotos)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task<User> GetUserAsync(Guid id)
        {
            return await _dataContext.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehiclePhotos)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id.ToString());
        }
        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }
        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            User currentUser = await GetUserAsync(user.Email);
            currentUser.LastName = user.LastName ?? currentUser.LastName;
            currentUser.FirstName = user.FirstName ?? currentUser.FirstName;
            currentUser.DocumentType = user.DocumentType?? currentUser.DocumentType;
            currentUser.Document = user.Document?? currentUser.Document;
            currentUser.Address = user.Address ?? currentUser.Address;
            currentUser.ImageId = user.ImageId;
            currentUser.PicturePath = user.PicturePath;
            currentUser.PhoneNumber = user.PhoneNumber ?? currentUser.PhoneNumber;
            return await _userManager.UpdateAsync(currentUser);
        }
        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }
        public async Task<User> AddUserAsync(AddUserViewModel model, Guid imageId, UserType userType)
        {
            User user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageId = imageId,
                PhoneNumber = model.PhoneNumber,
                DocumentType = await _dataContext.DocumentTypes.FindAsync(model.DocumentTypeId),
                UserName = model.Username,
                UserType = userType
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            User newUser = await GetUserAsync(model.Username);
            await AddUserToRoleAsync(newUser, user.UserType.ToString());
            return newUser;
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
