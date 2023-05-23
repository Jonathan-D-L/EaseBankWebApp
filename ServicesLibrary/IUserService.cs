using System.Security.Claims;
using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using UtilityLibrary;

namespace ServicesLibrary
{
    public interface IUserService
    {
        PagedResult<UserViewModel> GetUsers(ClaimsPrincipal user, string sortColumn, string sortOrder, int page, string q);
        Task<IdentityUser> GetUser(string id);
        Task<UserViewModel> GetUserForModel(string id);
        Task<IdentityResult> DeleteUser(IdentityUser user);
        Task<SelectList> GetRoles();
        Task<SelectList> GetRolesWithId();
        Task<string> GetUserRoles(IdentityUser user);
        Task<ErrorCode> ResetPassword(string id, string NewPassword);
        Task<ErrorCode> CreateUser(string email, string password, string role);
        Task<ErrorCode> UpdateUser(UserViewModel updateUser, string id, string updateRole);
        bool UserAlreadyExists(string email);
    }
}
