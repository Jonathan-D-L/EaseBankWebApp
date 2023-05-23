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
    public class CustomerEditModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerEditModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        public List<SelectListItem> Country { get; set; }
        public List<SelectListItem> Gender { get; set; }

        [BindProperty]
        public CustomerViewModel Customer { get; set; }
        public void OnGet(int customerId)
        {
            Country = GetListItems.GetCounties();

            Gender = GetListItems.GetGenders();

            var customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);
        }

        public IActionResult OnPostUpdate(int customerId)
        {
            Country = GetListItems.GetCounties();

            Gender = GetListItems.GetGenders();

            var customer = _mapper.Map<Customer>(Customer);
            customer.CustomerId = customerId;

            var status = _customerService.UpdateCustomer(customer);

            customer = _customerService.GetCustomer(customerId);
            Customer = _mapper.Map<CustomerViewModel>(customer);

            if (status == ErrorCode.Success)
            {
                TempData["Message"] = $"Client Account, {customer.Givenname} {customer.Surname} was updated successfully!";
                return RedirectToPage("/Customers/CustomerEdit", new { customerId });
            }

            if (status == ErrorCode.NoChangeOnSubmit)
            {
                ViewData["Message"] = "Your submission was invalid as no changes were made.";
            }

            if (status == ErrorCode.Error)
            {
                ModelState.AddModelError("Customer", "Something went wrong.");
            }

            return Page();
        }
    }
}
