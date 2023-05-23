using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Customers
{
    [Authorize(Roles = "Cashier,Admin")]
    public class CustomerModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;
        public CustomerModel(ICustomerService customerService, IAccountService accountService, ICardService cardService, IMapper mapper)
        {
            _customerService = customerService;
            _accountService = accountService;
            _cardService = cardService;
            _mapper = mapper;
        }
        public CustomerViewModel Customer { get; set; }
        public List<AccountsViewModel> Accounts { get; set; }

        public int AccountsCount { get; set; }
        public decimal AccountsSum { get; set; }

        public int CardsCount { get; set; }

        public int LoansCount { get; set; }
        public decimal LoansTotal { get; set; }

        public int PaymentsCount { get; set; }
        public int TransactionsCount { get; set; }

        public string Flag { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }

        public void OnGet(int customerId)
        {
           
            var cardsCount = _cardService.GetCardsCount(customerId);
            CardsCount = cardsCount;

            var accountsCount = _accountService.GetAccountsCount(customerId);
            AccountsCount = accountsCount;

            var accountsSum = _accountService.GetAccountsSum(customerId);
            AccountsSum = accountsSum;

            var loansTotal = _accountService.GetLoansSum(customerId);
            LoansTotal = loansTotal;

            var loansCount = _accountService.GetLoansCount(customerId);
            LoansCount = loansCount;

            var paymentsCount = _accountService.GetPaymentsCount(customerId);
            PaymentsCount = paymentsCount;

            var transactionsCount = _accountService.GetTransactionsCount(customerId);
            TransactionsCount = transactionsCount;

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);
           
            Customer.City = Format.FormatTitleCase(Customer.City);
            Customer.Gender = Format.FormatTitleCase(Customer.Gender);

            Flag = CountryCodeMapper.GetFlag(Customer.CountryCode);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            Country = CountryCodeMapper.GetCountry(Customer.CountryCode);
        }
    }
}
