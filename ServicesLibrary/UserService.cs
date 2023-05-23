using System.Security.Claims;
using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityLibrary;

namespace ServicesLibrary
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public PagedResult<UserViewModel> GetUsers(ClaimsPrincipal user, string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "UserName";
                sortOrder = "asc";
            }
            var currentUser = _userManager.GetUserId(user);
            var users = _userManager.Users
                .Where(u => u.Id != currentUser)
                .ToList()
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                })
                .AsQueryable()
                .SortColumn(sortColumn, sortOrder);

            if (!string.IsNullOrWhiteSpace(q))
            {
                users = Search.ScopedDataSearch(users, q).AsQueryable();
            }
          
            return users.GetPaged(page, 5);
        }
        public async Task<UserViewModel> GetUserForModel(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }
        public async Task<IdentityUser> GetUser(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<IdentityResult> DeleteUser(IdentityUser user)
        {
            return await _userManager.DeleteAsync(user);
        }
        public async Task<SelectList> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new SelectList(roles, "Name");
        }
        public async Task<SelectList> GetRolesWithId()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new SelectList(roles,"Id", "Name");
        }
        public async Task<string> GetUserRoles(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            return userRoles.Count > 0 ? (await _roleManager.FindByNameAsync(userRoles[0])).Id : "";
        }
        public bool UserAlreadyExists(string email)
        {
            return _userManager.FindByEmailAsync(email).Result != null;
        }
        public async Task<ErrorCode> CreateUser(string email, string password, string role)
        {
            if (string.IsNullOrEmpty(role)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(role)) { return ErrorCode.Error; }

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) { return ErrorCode.Error; }
            await _userManager.AddToRoleAsync(user, role);
            return ErrorCode.Success;
        }
        public async Task<ErrorCode> UpdateUser(UserViewModel updateUser, string id, string updateRole)
        {
            if (string.IsNullOrEmpty(updateUser.Email)
                || string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(updateRole)) { return ErrorCode.Error; }

            var user = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var userUpdateRoles = await _roleManager.FindByIdAsync(updateRole);

            if (user == null) { return ErrorCode.Error; }

            if (user.PhoneNumber == updateUser.PhoneNumber 
                && user.Email == updateUser.Email 
                && userRoles.FirstOrDefault() == userUpdateRoles.Name) { return ErrorCode.NoChangeOnSubmit; }


            user.Email = updateUser.Email;
            user.UserName = updateUser.Email;
            user.PhoneNumber = updateUser.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) { return ErrorCode.Error; }

            var role = await _roleManager.FindByIdAsync(updateRole);
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            await _userManager.AddToRoleAsync(user, role.Name);

            return ErrorCode.Success;
        }
        public async Task<ErrorCode> ResetPassword(string id, string NewPassword)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(NewPassword)) { return ErrorCode.Error; }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) { return ErrorCode.Error; }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, NewPassword);

            return !result.Succeeded ? ErrorCode.Error : ErrorCode.Success;
        }
    }
}
