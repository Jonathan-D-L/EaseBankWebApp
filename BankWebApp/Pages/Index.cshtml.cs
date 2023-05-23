using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicesLibrary;

namespace BankWebApp.Pages
{
    public partial class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;

        public IndexModel(ICustomerService customerService ,IAccountService accountService)
        {
            _customerService = customerService;
            _accountService = accountService;
        }

        public CustomerDataViewModel CustomerDataSE { get; set; } = new();
        public CustomerDataViewModel CustomerDataNO { get; set; } = new();
        public CustomerDataViewModel CustomerDataDK { get; set; } = new();
        public CustomerDataViewModel CustomerDataFI { get; set; } = new();
        public void OnGet()
        {
            CustomerDataSE.TotalCustomersSE = _customerService.GetCustomerPerCountry("NO");
            CustomerDataNO.TotalCustomersNO = _customerService.GetCustomerPerCountry("NO");
            CustomerDataDK.TotalCustomersDK = _customerService.GetCustomerPerCountry("DK");
            CustomerDataFI.TotalCustomersFI = _customerService.GetCustomerPerCountry("FI");

            CustomerDataSE.TotalAccountsSE = _customerService.GetCustomerAccountsPerCountry("SE");
            CustomerDataNO.TotalAccountsNO = _customerService.GetCustomerAccountsPerCountry("NO");
            CustomerDataDK.TotalAccountsDK = _customerService.GetCustomerAccountsPerCountry("DK");
            CustomerDataFI.TotalAccountsFI = _customerService.GetCustomerAccountsPerCountry("FI");

            CustomerDataSE.TotalAccountsValueSE = _accountService.GetAccountTotalValuePerCountry("SE");
            CustomerDataNO.TotalAccountsValueNO = _accountService.GetAccountTotalValuePerCountry("NO");
            CustomerDataDK.TotalAccountsValueDK = _accountService.GetAccountTotalValuePerCountry("DK");
            CustomerDataFI.TotalAccountsValueFI = _accountService.GetAccountTotalValuePerCountry("FI");
        }
    }
}