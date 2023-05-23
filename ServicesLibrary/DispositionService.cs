using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ServicesLibrary
{
    public class DispositionService : IDispositionService
    {
        private readonly BankAppDataContext _dbContext;
        public DispositionService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<Disposition> GetDispositions(string countryCode)
        {
            var dispositions = _dbContext.Dispositions
                .Include(c => c.Customer)
                .Include(a => a.Account)
                .Where(c => c.Customer.CountryCode == countryCode)
                .Where(t => t.Type.ToLower() == "owner")
                .OrderByDescending(b => b.Account.Balance)
                .Take(10);
            return dispositions;
        }
    
        public List<Disposition> GetAccountAndHolderName(string? q)
        {
            if (q == null)
            {
                return new List<Disposition>();
            }

            var accounts = _dbContext.Dispositions
                .Include(a => a.Account)
                .Include(c => c.Customer)
                .Where(d => d.Type.ToLower() == "owner");

            var searchTerms = q.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = accounts;

            foreach (var term in searchTerms)
            {
                if (searchTerms.Length == searchTerms.Length - 1 || result.Any())
                {
                    result = accounts.Where(c =>
                        c.Account.AccountId.ToString().StartsWith(term) ||
                        c.Customer.Givenname.StartsWith(term) ||
                        c.Customer.Surname.StartsWith(term));
                }
                else
                {
                    result = result.Where(c =>
                        c.Account.AccountId.ToString() == term ||
                        c.Customer.Givenname == term ||
                        c.Customer.Surname == term);
                }
            }
            return result.ToList();
        }
    }
}
