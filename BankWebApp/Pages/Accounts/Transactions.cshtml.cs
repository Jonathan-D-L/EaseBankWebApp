using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Accounts
{
    [Authorize(Roles = "Cashier,Admin")]
    public class TransactionsModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public TransactionsModel(ICustomerService customerService, IAccountService accountService, IMapper mapper)
        {
            _customerService = customerService;
            _accountService = accountService;
            _mapper = mapper;
        }
        public CustomerViewModel Customer { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; }
        public string Currency { get; set; }
        public int CustomerId { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string Q { get; set; }

        public void OnGet(int customerId,string sortColumn,string sortOrder, int pageNum, string q)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            SortColumn = sortColumn;

            SortOrder = sortOrder;

            Q = q;

            var transactions = _accountService.GetTransactions(customerId, sortColumn, sortOrder, pageNum, q);
            Transactions = _mapper.Map<List<TransactionsViewModel>>(transactions.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            CustomerId = customerId;

            PageCount = transactions.PageCount;
        }
    }
}
