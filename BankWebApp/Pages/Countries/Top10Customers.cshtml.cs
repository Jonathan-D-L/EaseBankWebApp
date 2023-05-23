using AutoMapper;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Countries
{
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "countryCode" })]
    public class Top10CustomersModel : PageModel
    {
        private readonly IDispositionService _dispositionService;
        private readonly IMapper _mapper;
        public Top10CustomersModel(IDispositionService dispositionService, IMapper mapper)
        {
            _dispositionService = dispositionService;
            _mapper = mapper;
        }
        public List<Top10CustomerViewModel> Top10Customers { get; set; }
        public string Flag { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public void OnGet(string countryCode)
        {
            var top10Customers = _dispositionService.GetDispositions(countryCode);
            Top10Customers = _mapper.Map<List<Top10CustomerViewModel>>(top10Customers);
            Flag = CountryCodeMapper.GetFlag(countryCode);
            Currency = CountryCodeMapper.GetCurrency(countryCode);
            Country = CountryCodeMapper.GetCountry(countryCode);
        }
    }
}
