using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Accounts
{
    [Authorize(Roles = "Cashier,Admin")]
    public class CardsModel : PageModel
    {

        private readonly ICustomerService _customerService;
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;

        public CardsModel(ICardService cardService, ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _cardService = cardService;
            _mapper = mapper;
        }
        public CustomerViewModel Customer { get; set; }
        public List<CardsViewModel> Cards { get; set; }
        public string Currency { get; set; }
        public int CustomerId { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string Q { get; set; }

        public void OnGet(int customerId, string sortColumn, string sortOrder, int pageNum, string q)
        {
            if (pageNum == 0)
            {
                pageNum = 1;
            }

            CurrentPage = pageNum;

            SortColumn = sortColumn;

            SortOrder = sortOrder;

            Q = q;

            var cards = _cardService.GetCards(customerId, sortColumn, sortOrder, pageNum, q);
            Cards = _mapper.Map<List<CardsViewModel>>(cards.Results);

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            Currency = CountryCodeMapper.GetCurrency(Customer.CountryCode);

            CustomerId = customerId;

            PageCount = cards.PageCount;

            foreach (var card in Cards)
            {
                card.Type = Format.FormatTitleCase(card.Type);
                card.ExpM = Format.FormatIntToDate(card.ExpM);
            }
        }
    }
}
