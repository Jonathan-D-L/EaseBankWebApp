using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class UserEditModel : PageModel
    {
        private readonly IUserService _userService;
        public UserEditModel(IUserService userService)
        {
            _userService = userService;
        }
        public SelectList RoleList { get; set; }

        [BindProperty]
        public new IdentityUser User { get; set; }

        [BindProperty]
        public UserViewModel UpdateUser { get; set; }

        [BindProperty]
        public string Role { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            User = await _userService.GetUser(id);

            UpdateUser = await _userService.GetUserForModel(id);

            if (UpdateUser == null || User == null)
            {
                return NotFound();
            }

            RoleList = await _userService.GetRolesWithId();

            Role = await _userService.GetUserRoles(User);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            User = await _userService.GetUser(id);

            var status = await _userService.UpdateUser(UpdateUser, id, Role);

            RoleList = await _userService.GetRolesWithId();

            Role = await _userService.GetUserRoles(User);

            if (status == ErrorCode.Success)
            {
                TempData["MessageGreen"] = $"User {User.Email} was Updated successfully!";
                return RedirectToPage("/Users/Users");
            }

            if (status == ErrorCode.NoChangeOnSubmit)
            {
                ViewData["Message"] = "Your submission was invalid as no changes were made.";
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("User", "Something went wrong.");
            }

            return Page();
        }

    }

}
