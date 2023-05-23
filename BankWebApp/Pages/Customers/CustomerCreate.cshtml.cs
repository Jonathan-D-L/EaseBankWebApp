using AutoMapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesLibrary;
using UtilityLibrary;

namespace BankWebApp.Pages.Customers
{
    [Authorize(Roles = "Cashier,Admin")]
    public class CustomerCreateModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerCreateModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        public List<SelectListItem> Country { get; set; }
        public List<SelectListItem> Gender { get; set; }

        [BindProperty] 
        public CustomerViewModel Customer { get; set; }
        public void OnGet()
        {
            Country = GetListItems.GetCounties();

            Gender = GetListItems.GetGenders();
        }
        public IActionResult OnPostCreate()
        {
            var customer = _mapper.Map<Customer>(Customer);
            var status = _customerService.CreateCustomer(customer);

            Country = GetListItems.GetCounties();

            Gender = GetListItems.GetGenders();
            if (status == ErrorCode.Success)
            {
                TempData["Message"] = $"Client Account for {customer.Givenname} {customer.Surname} was created successfully!";
                return RedirectToPage("/Customers/CustomerCreate");
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("Customer", "Something went wrong.");
            }

            return Page();
        }
    }
}
