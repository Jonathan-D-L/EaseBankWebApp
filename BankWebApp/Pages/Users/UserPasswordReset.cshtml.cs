using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using System.ComponentModel.DataAnnotations;
using UtilityLibrary;

namespace BankWebApp.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class UserPasswordResetModel : PageModel
    {
        private readonly IUserService _userService;
        public UserPasswordResetModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public new IdentityUser User { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", 
            ErrorMessage = "Password must be at least 8 characters with at least 1 digit, lowercase, uppercase, and special character.")]
        public string NewPassword { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            User = await _userService.GetUser(id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            var status = await _userService.ResetPassword(id, NewPassword);

            if (status == ErrorCode.Success)
            {
                TempData["MessageGreen"] = $"User {User.Email} password was reset successfully!";
                return RedirectToPage("/Users/Users");
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("User", "Something went wrong.");
            }

            return Page();
        }
    }
}
