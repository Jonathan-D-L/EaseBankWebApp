using AutoMapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesLibrary;
using UtilityLibrary;


namespace BankWebApp.Pages.Accounts
{
    [Authorize(Roles = "Cashier,Admin")]
    public class AccountCreateModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountCreateModel(ICustomerService customerService, IAccountService accountService, IMapper mapper)
        {
            _customerService = customerService;
            _accountService = accountService;
            _mapper = mapper;
        }

        public CustomerViewModel Customer { get; set; }
        public List<SelectListItem> DisponentOptions { get; set; }

        public List<SelectListItem> Frequencies { get; set; }

        public string Q { get; set; }

        [BindProperty]
        public CustomerViewModel Disponent { get; set; }

        [BindProperty]
        public string Frequency { get; set; }

        [BindProperty] 
        public int DisponentId { get; set; }

        public void OnGet(int customerId)
        {
            Frequencies = GetListItems.GetFrequency();
            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);
        }

        public IActionResult OnPostCreate(int customerId)
        {
            Frequencies = GetListItems.GetFrequency();
            Customer disponent = new();
            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);
            if (DisponentId != 0)
            {
                disponent = _customerService.GetCustomer(DisponentId);
                Disponent = _mapper.Map<CustomerViewModel>(disponent);
            }
            var status = _accountService.CreateAccount(customerId, DisponentId, Frequency);
            if (status == ErrorCode.Success)
            {
                if (DisponentId != 0)
                {
                    TempData["successMessage"] = $"Client Account for ' {customer.Givenname} {customer.Surname} ' was created successfully!" +
                                          $" and a management account has been created for ' {disponent.Givenname} {disponent.Surname} '";
                }
                else
                {
                    TempData["successMessage"] = $"Client Account for ' {customer.Givenname} {customer.Surname} ' was created successfully!";
                }
                return RedirectToPage("/Accounts/Accounts", new { customerId });
            }

            if (status == ErrorCode.Error)
            {
                ViewData["Message"] = $"There was an error when trying to create the account please try again later.";
                ModelState.AddModelError("Account", "Something went wrong.");
            }
            return Page();
        }

        public IActionResult OnGetFetchInfo(string q)
        {
            var disponents = _customerService.GetCustomerForDisposition(q);

            var disponentOptions = disponents.Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = $"{x.Givenname} {x.Surname} - Id {x.CustomerId}"
            }).ToList();

            return new JsonResult(disponentOptions);
        }
    }
}
