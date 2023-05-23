using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesLibrary;
using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Infrastructure.Validation;
using UtilityLibrary;

namespace BankWebApp.Pages.Accounts
{
    [Authorize(Roles = "Cashier,Admin")]
    public class AccountModel : PageModel
    {
        private readonly IDispositionService _dispositionService;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountModel(IDispositionService dispositionService, ICustomerService customerService, IAccountService accountService, IMapper mapper)
        {
            _dispositionService = dispositionService;
            _customerService = customerService;
            _accountService = accountService;
            _mapper = mapper;
        }

        public CustomerViewModel Customer { get; set; }
        public AccountsViewModel Account { get; set; }
        public List<SelectListItem> AccountOptions { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; }
        public Action Action { get; set; }
        public string Q { get; set; }
        public string Currency { get; set; }

        [BindProperty]
        [CustomDecimal] 
        [Range(0, 1000000.00)]
        public decimal Amount { get; set; }

        [BindProperty] 
        public int ToAccountId { get; set; }

        public void OnGet(int customerId, int accountId, Action action, int pageNum)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            var transactions = _accountService.GetTransactionsForAccount(customerId, accountId, pageNum);
            Transactions = _mapper.Map<List<TransactionsViewModel>>(transactions.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            var account = _accountService.GetAccount(accountId);
            Account = _mapper.Map<AccountsViewModel>(account);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            Action = action;
        }

        public IActionResult OnPostTransfer(int customerId, int accountId, int pageNum)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            var status = _accountService.Transaction(accountId, ToAccountId, Amount);

            var transactions = _accountService.GetTransactionsForAccount(customerId, accountId, pageNum);
            Transactions = _mapper.Map<List<TransactionsViewModel>>(transactions.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            var account = _accountService.GetAccount(accountId);
            Account = _mapper.Map<AccountsViewModel>(account);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            Action = Action.Transfer;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your transaction of {Amount} {Currency}. Has been processed successfully!";
                return RedirectToPage("/Accounts/Account", new { accountId, customerId, Action });
            }

            if (status == ErrorCode.AmountOutOfRange)
            {
                ModelState.AddModelError("Amount", "Please enter an amount between 100 and 1,000,000.");
            }

            if (status == ErrorCode.InvalidAccount)
            {
                ModelState.AddModelError("ToAccountId", "Invalid Account");
            }

            return Page();
        }

        public IActionResult OnPostWithdraw(int accountId, int customerId)
        {
            var status = _accountService.Withdraw(accountId, Amount);

            var account = _accountService.GetAccount(accountId);
            Account = _mapper.Map<AccountsViewModel>(account);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            Action = Action.Withdraw;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your withdrawal of -{Amount} {Currency}. Has been processed successfully!";
                return RedirectToPage("/Accounts/Account", new { accountId, customerId, Action });
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
        public IActionResult OnPostDeposit(int accountId, int customerId)
        {
            var status = _accountService.Deposit(accountId, Amount);

            var account = _accountService.GetAccount(accountId);
            Account = _mapper.Map<AccountsViewModel>(account);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);
            Action = Action.Deposit;

            if (ModelState.IsValid && status == ErrorCode.Success)
            {
                TempData["Message"] = $"Your deposit of {Amount} {Currency}. Has been processed successfully!";
                return RedirectToPage("/Accounts/Account", new { accountId, customerId, Action });
            }

            if (status == ErrorCode.AmountOutOfRange)
            {
                ModelState.AddModelError("Amount", "Please enter an amount between 100 and 1,000,000.");
            }

            return Page();
        }

        public IActionResult OnGetFetchInfo(string q)
        {
            var holderAndAccounts = _dispositionService.GetAccountAndHolderName(q);

            var accountOptions = holderAndAccounts.Select(x => new SelectListItem
            {
                Value = x.AccountId.ToString(),
                Text = $"{x.Customer.Givenname} {x.Customer.Surname} - Account {x.Account.AccountId}"
            }).ToList();

            return new JsonResult(accountOptions);
        }

        public IActionResult OnGetShowMore(int customerId, int accountId, int pageNum)
        {
            var transactions = _accountService.GetTransactionsForAccount(customerId, accountId, pageNum);
            Transactions = _mapper.Map<List<TransactionsViewModel>>(transactions.Results);

            return new JsonResult(new { Transactions });
        }
    }
}
