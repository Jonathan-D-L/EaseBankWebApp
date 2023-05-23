using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesLibrary;
using System.ComponentModel.DataAnnotations;
using UtilityLibrary;

namespace BankWebApp.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class UserCreateModel : PageModel
    {
        private readonly IUserService _userService;

        public UserCreateModel(IUserService userService)
        {
            _userService = userService;
        }
        public SelectList RoleList { get; set; }

        [BindProperty] 
        public new UserViewModel User { get; set; }

        [BindProperty] 
        public string Role { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", 
            ErrorMessage = "Password must be at least 8 characters with at least 1 digit, lowercase, uppercase, and special character.")]
        public string Password { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            RoleList = await _userService.GetRoles();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            RoleList = await _userService.GetRoles();

            if (_userService.UserAlreadyExists(User.Email)) return Page();

            var status = await _userService.CreateUser(User.Email, Password, Role);

            if (status == ErrorCode.Success)
            {
                TempData["MessageGreen"] = $"User {User.Email} was created successfully!";
                return RedirectToPage("/Users/Users", new { TempData });
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("User", "Something went wrong.");
            }

            return Page();
        }

    }
}
