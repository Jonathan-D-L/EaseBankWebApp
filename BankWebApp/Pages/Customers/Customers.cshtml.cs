using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Customers
{
    [Authorize(Roles = "Cashier,Admin")]
    public class CustomersModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly  IMapper _mapper;
        public CustomersModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string Q { get; set; }

        public List<CustomerViewModel> Customer { get; set; }
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

            var customers = _customerService.GetCustomers(sortColumn, sortOrder, pageNum, q);
            Customer = _mapper.Map<List<CustomerViewModel>>(customers.Results);

            PageCount = customers.PageCount;

            foreach (var cust in Customer)
            {
                cust.City = Format.FormatTitleCase(cust.City);
            }
        }
    }
}

