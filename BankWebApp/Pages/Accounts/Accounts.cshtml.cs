using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Infrastructure.Validation;
using UtilityLibrary;

namespace BankWebApp.Pages.Accounts
{
    public enum Action
    {
        None,
        Deposit,
        Withdraw,
        Transfer,
        Delete
    }
    [Authorize(Roles = "Cashier,Admin")]
    public class AccountsModel : PageModel
    {

        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AccountsModel(ICustomerService customerService, IAccountService accountService, IMapper mapper)
        {
            _customerService = customerService;
            _accountService = accountService;
            _mapper = mapper;
        }
        public CustomerViewModel Customer { get; set; }
        public List<AccountsViewModel> Accounts { get; set; }
        public int AccountId { get; set; }
        public string Currency { get; set; }
        public Action Action { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }

        [BindProperty]
        [CustomDecimal]
        [Range(0, 1000000.00)]
        public decimal Amount { get; set; }

        public void OnGet(int customerId, int accountId, Action action, int pageNum)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            var accounts = _accountService.GetAccounts(customerId, pageNum);
            Accounts = _mapper.Map<List<AccountsViewModel>>(accounts.Results);
            
            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            PageCount = accounts.PageCount;

            AccountId = accountId;

            Action = action;
        }


        public IActionResult OnPostWithdraw(int accountId, int customerId, int pageNum)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            var status = _accountService.Withdraw(accountId, Amount);

            var accounts = _accountService.GetAccounts(customerId, pageNum);
            Accounts = _mapper.Map<List<AccountsViewModel>>(accounts.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            AccountId = accountId;

            Action = Action.Withdraw;

            PageCount = accounts.PageCount;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your withdrawal of -{Amount} {Currency}. Has been processed successfully!";
                return RedirectToPage("/Accounts/Accounts", new { accountId, customerId, Action, pageNum, PageCount });
            }

            if (status == ErrorCode.DeficitBalance)
            {
                ModelState.AddModelError("Amount", "Insufficient founds.");
            }

            if (status == ErrorCode.AmountOutOfRange)
            {
                ModelState.AddModelError("Amount", "Please enter an amount between 100 and 1,000,000.");
            }

            return Page();
        }
        public IActionResult OnPostDeposit(int accountId, int customerId, int pageNum)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            var status = _accountService.Deposit(accountId, Amount);

            var accounts = _accountService.GetAccounts(customerId, pageNum);
            Accounts = _mapper.Map<List<AccountsViewModel>>(accounts.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            AccountId = accountId;

            Action = Action.Deposit;

            PageCount = accounts.PageCount;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your deposit of {Amount} {Currency}. Has been processed successfully!";
                return RedirectToPage("/Accounts/Accounts", new { accountId, customerId, Action, pageNum, PageCount });
            }

            if (status == ErrorCode.AmountOutOfRange)
            {
                ModelState.AddModelError("Amount", "Please enter an amount between 100 and 1,000,000.");
            }

            return Page();
        }

        public IActionResult OnPostDelete(int accountId, int customerId, int pageNum, string type)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            var status = _accountService.DeleteAccount(accountId, type);

            var accounts = _accountService.GetAccounts(customerId, pageNum);
            Accounts = _mapper.Map<List<AccountsViewModel>>(accounts.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            AccountId = accountId;

            Action = Action.Delete;

            PageCount = accounts.PageCount;

            CurrentPage = pageNum;

            pageNum = accounts.PageCount;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your Account ({AccountId}) has been deleted successfully!";
                return RedirectToPage("/Accounts/Accounts", new { customerId, Action, pageNum, PageCount });
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("AccountId", "Something went wrong.");
            }

            return Page();
        }
    }
}
