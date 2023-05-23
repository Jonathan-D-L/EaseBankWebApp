using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;
using UtilityLibrary;

namespace ServicesLibrary
{
    public interface ICustomerService
    {
        int GetCustomerPerCountry(string country);
        int GetCustomerAccountsPerCountry(string country);
      
        Customer GetCustomer(int customerId);
        List<Customer> GetCustomers();
        List<Customer> GetCustomerForDisposition(string q);
        PagedResult<Customer> GetCustomers(string sortColumn, string sortOrder, int pageNum, string q);
        
        Task<Customer> GetCustomerAsync(int customerId);
        
        ErrorCode CreateCustomer(Customer customer);
        ErrorCode UpdateCustomer(Customer customer);
    }
}
