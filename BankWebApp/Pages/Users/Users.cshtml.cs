using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;

namespace BankWebApp.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;
        public UsersModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public IList<UserViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string Q { get; set; }

        public void OnGet(string sortColumn, string sortOrder, int pageNum, string q)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            SortColumn = sortColumn;

            SortOrder = sortOrder;

            Q = q;

            var users = _userService.GetUsers(User, sortColumn, sortOrder, pageNum, q);

            Users = users.Results;

            PageCount = users.PageCount;
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userService.DeleteUser(user);

            if (result.Succeeded)
            {
                TempData["MessageRed"] = $"User ({user.UserName}) has been deleted successfully!";
                return RedirectToPage("/Users/Users");
            }

            return Page();
        }
    }
}
