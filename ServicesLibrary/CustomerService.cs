using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using UtilityLibrary;

namespace ServicesLibrary
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _dbContext;
        public CustomerService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int GetCustomerPerCountry(string country)
        {
            var customers = _dbContext.Customers
                .Count(c => c.CountryCode
                    .ToLower() == country);
            return customers;
        }
        public int GetCustomerAccountsPerCountry(string country)
        {
            var accounts = _dbContext.Dispositions
                .Include(c => c.Customer)
                .Include(a => a.Account)
                .Where(c => c.Customer.CountryCode == country)
                .Count(d => d.Type.ToLower() == "owner");
            return accounts;
        }

        public Customer GetCustomer(int customerId)
        {
            return _dbContext.Customers.First(c => c.CustomerId == customerId);
        }
        public List<Customer> GetCustomers()
        {
            return _dbContext.Customers.OrderBy(c => c.Givenname).ToList();
        }
        public List<Customer> GetCustomerForDisposition(string? q)
        {
            if (q == null)
            {
                return new List<Customer>();
            }

            var customer = _dbContext.Customers.AsQueryable();

            var searchTerms = q.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = customer;

            foreach (var term in searchTerms)
            {
                if (searchTerms.Length == searchTerms.Length - 1 || result.Any())
                {
                    result = customer.Where(c =>
                        c.CustomerId.ToString().StartsWith(term) ||
                        c.Givenname.StartsWith(term) ||
                        c.Surname.StartsWith(term));
                }
                else
                {
                    result = result.Where(c =>
                        c.CustomerId.ToString() == term ||
                        c.Givenname == term ||
                        c.Surname == term);
                }
            }
            return result.ToList();
        }

        public PagedResult<Customer> GetCustomers(string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "Givenname";
                sortOrder = "asc";
            }
            var customers = _dbContext.Customers.SortColumn(sortColumn, sortOrder);
            if (!string.IsNullOrWhiteSpace(q))
            {
                customers = Search.ScopedDataSearch(customers, q).AsQueryable();
            }
            return customers.GetPaged(page, 10);
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            return await _dbContext.Customers
                .Include(d=>d.Dispositions.Where(a=>a.CustomerId == customerId))
                .FirstAsync(c => c.CustomerId == customerId);
        }

        public ErrorCode CreateCustomer(Customer customer)
        {
            if (customer == null
                || string.IsNullOrEmpty(customer.Givenname)
                || string.IsNullOrEmpty(customer.Surname)
                || string.IsNullOrEmpty(customer.Streetaddress)
                || string.IsNullOrEmpty(customer.City)
                || string.IsNullOrEmpty(customer.Country)
                || customer.Birthday == null
                || string.IsNullOrEmpty(customer.Telephonenumber)
                || string.IsNullOrEmpty(customer.Emailaddress)) { return ErrorCode.Error; }

            customer.Givenname = Format.FormatTitleCase(customer.Givenname);
            customer.Surname = Format.FormatTitleCase(customer.Surname);
            customer.City = Format.FormatTitleCase(customer.City);
            customer.Streetaddress = Format.FormatTitleCase(customer.Streetaddress);
            switch (customer.Country.ToLower())
            {
                case "sweden":
                    customer.CountryCode = "SE";
                    customer.Telephonecountrycode = "46";
                    break;
                case "denmark":
                    customer.CountryCode = "DK";
                    customer.Telephonecountrycode = "45";
                    break;
                case "norway":
                    customer.CountryCode = "NO";
                    customer.Telephonecountrycode = "47";
                    break;
                case "finland":
                    customer.CountryCode = "FI";
                    customer.Telephonecountrycode = "358";
                    break;
            }
            var newCustomer = new Customer
            {
                Gender = customer.Gender,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Zipcode = customer.Zipcode,
                Country = customer.Country,
                CountryCode = customer.CountryCode,
                Birthday = customer.Birthday,
                NationalId = customer.NationalId,
                Telephonecountrycode = customer.Telephonecountrycode,
                Telephonenumber = customer.Telephonenumber,
                Emailaddress = customer.Emailaddress
            };
            var account = new Account
            {
                Frequency = "Monthly",
                Created = DateTime.Now,
                Balance = 0
            };
            _dbContext.AddRange(newCustomer, account);
            _dbContext.SaveChanges();
            var newCustomerId = newCustomer.CustomerId;
            var newAccountId = account.AccountId;
            var disposition = new Disposition
            {
                CustomerId = newCustomerId,
                AccountId = newAccountId,
                Type = "OWNER"
            };
            _dbContext.Add(disposition);
            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
        public ErrorCode UpdateCustomer(Customer customer)
        {
            var customerToUpdate = _dbContext.Customers
                .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

            if (customerToUpdate == null
                || customer == null
                || string.IsNullOrEmpty(customer.Givenname)
                || string.IsNullOrEmpty(customer.Surname)
                || string.IsNullOrEmpty(customer.Streetaddress)
                || string.IsNullOrEmpty(customer.City)
                || string.IsNullOrEmpty(customer.Country)
                || customer.Birthday == null
                || string.IsNullOrEmpty(customer.Telephonenumber)
                || string.IsNullOrEmpty(customer.Emailaddress)) { return ErrorCode.Error; }

            customer.Givenname = Format.FormatTitleCase(customer.Givenname);
            customer.Surname = Format.FormatTitleCase(customer.Surname);
            customer.City = Format.FormatTitleCase(customer.City);
            customer.Streetaddress = Format.FormatTitleCase(customer.Streetaddress);
            switch (customer.Country.ToLower())
            {
                case "sweden":
                    customer.CountryCode = "SE";
                    customer.Telephonecountrycode = "46";
                    break;
                case "denmark":
                    customer.CountryCode = "DK";
                    customer.Telephonecountrycode = "45";
                    break;
                case "norway":
                    customer.CountryCode = "NO";
                    customer.Telephonecountrycode = "47";
                    break;
                case "finland":
                    customer.CountryCode = "FI";
                    customer.Telephonecountrycode = "358";
                    break;
            }

            if (ObjectComparer.AreObjectsEqual(customer, customerToUpdate))
            {
                return ErrorCode.NoChangeOnSubmit;
            }

            customerToUpdate.Gender = customer.Gender;
            customerToUpdate.Givenname = customer.Givenname;
            customerToUpdate.Surname = customer.Surname;
            customerToUpdate.Streetaddress = customer.Streetaddress;
            customerToUpdate.City = customer.City;
            customerToUpdate.Zipcode = customer.Zipcode;
            customerToUpdate.Country = customer.Country;
            customerToUpdate.CountryCode = customer.CountryCode;
            customerToUpdate.Birthday = customer.Birthday;
            customerToUpdate.NationalId = customer.NationalId;
            customerToUpdate.Telephonecountrycode = customer.Telephonecountrycode;
            customerToUpdate.Telephonenumber = customer.Telephonenumber;
            customerToUpdate.Emailaddress = customer.Emailaddress;

            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
    }
}
