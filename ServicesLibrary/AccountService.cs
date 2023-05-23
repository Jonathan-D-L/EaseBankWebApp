using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using UtilityLibrary;

namespace ServicesLibrary
{
    public class AccountService : IAccountService
    {

        private readonly BankAppDataContext _dbContext;
        public AccountService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int GetAccountsCount(int customerId)
        {
            var accountsCount = _dbContext.Accounts
                .Include(a => a.Dispositions)
                .Where(a => a.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return accountsCount.Count();
        }
        public decimal GetAccountsSum(int customerId)
        {
            var accountsSum = _dbContext.Accounts
                .Include(a => a.Dispositions)
                .Where(a => a.Dispositions
                    .Any(d => d.CustomerId == customerId && d.Type == "owner"))
                .Select(a => a.Balance)
                .AsQueryable();
            return accountsSum.Sum();
        }
        public int GetLoansCount(int customerId)
        {
            var loansCount = _dbContext.Loans
                .Include(a => a.Account).ThenInclude(d => d.Dispositions)
                .Where(a => a.Account.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return loansCount.Count();
        }
        public decimal GetLoansSum(int customerId)
        {
            var loansTotal = _dbContext.Loans
                .Include(a => a.Account).ThenInclude(d => d.Dispositions)
                .Where(a => a.Account.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return loansTotal.Sum(s => s.Amount);
        }
        public int GetPaymentsCount(int customerId)
        {
            var loansCount = _dbContext.PermenentOrders
                .Include(a => a.Account).ThenInclude(d => d.Dispositions)
                .Where(a => a.Account.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return loansCount.Count();
        }
        public int GetTransactionsCount(int customerId)
        {
            var loansCount = _dbContext.Transactions
                .Include(a => a.AccountNavigation).ThenInclude(d => d.Dispositions)
                .Where(a => a.AccountNavigation.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return loansCount.Count();
        }
        public decimal GetAccountTotalValuePerCountry(string country)
        {
            var accountTotal = _dbContext.Dispositions
                .Include(c => c.Customer)
                .Include(a => a.Account)
                .Where(d => d.Customer.CountryCode == country)
                .Select(d => d.Account.Balance).Sum();
            return accountTotal;
        }

        public Account GetAccount(int accountId)
        {
            var account = _dbContext.Accounts.Include(d => d.Dispositions)
                .First(a => a.AccountId == accountId);
            return account;
        }
        public void UpdateAccount(Account account)
        {
            _dbContext.SaveChanges();
        }

        public PagedResult<Account> GetAccounts(int customerId, int page)
        {
            var accounts = _dbContext.Accounts
                .Include(a => a.Dispositions.Where(x => x.CustomerId == customerId))
                .Where(a => a.Dispositions.Any(d => d.CustomerId == customerId))
                .AsQueryable();
            return accounts.GetPaged(page, 4);
        }
        public PagedResult<Loan> GetLoans(int customerId, string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "Date";
                sortOrder = "desc";
            }
            var loans = _dbContext.Dispositions.Include(d => d.Customer)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(a => a.Account.Loans)
                .SortColumn(sortColumn, sortOrder);

            if (!string.IsNullOrWhiteSpace(q))
            {
                loans = Search.ScopedDataSearch(loans, q).AsQueryable();
            }
            return loans.GetPaged(page, 2);
        }
        public PagedResult<PermenentOrder> GetPermanentOrders(int customerId, string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "OrderId";
                sortOrder = "desc";
            }
            var permanentOrders = _dbContext.Dispositions.Include(d => d.Customer)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(a => a.Account.PermenentOrders)
                .SortColumn(sortColumn, sortOrder);
            if (!string.IsNullOrWhiteSpace(q))
            {
                permanentOrders = Search.ScopedDataSearch(permanentOrders, q).AsQueryable();
            }
            return permanentOrders.GetPaged(page, 10);
        }
        public List<Transaction> GetTransactions(int customerId)
        {
            return _dbContext.Dispositions.Include(d => d.Customer)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(a => a.Account.Transactions)
                .Where(d => d.Date >= DateTime.Now.AddDays(-7))
                .OrderByDescending(t => t.TransactionId)
                .AsNoTracking()
                .ToList();
        }
        public PagedResult<Transaction> GetTransactions(int customerId, string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "TransactionId";
                sortOrder = "desc";
            }
            var transaction = _dbContext.Dispositions.Include(d => d.Customer)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(a => a.Account.Transactions)
                .SortColumn(sortColumn, sortOrder);

            if (!string.IsNullOrWhiteSpace(q))
            {
                transaction = Search.ScopedDataSearch(transaction, q).AsQueryable();
            }
            return transaction.GetPaged(page, 10);
        }
        public PagedResult<Transaction> GetTransactionsForAccount(int customerId, int accountId, int page)
        {
            var accounts = _dbContext.Transactions
                .Include(a => a.AccountNavigation).ThenInclude(d => d.Dispositions)
                .Where(a => a.AccountNavigation.Dispositions.Any(d => d.CustomerId == customerId))
                .Where(a => a.AccountId == accountId)
                .OrderByDescending(d => d.TransactionId)
                .AsQueryable();

            return accounts.GetPaged(page, 10);
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int customerId, int accountId, int limit, int offset)
        {
            return await _dbContext.Dispositions
                .Include(d => d.Customer)
                .Include(a => a.Account)
                .Where(c => c.CustomerId == customerId)
                .Where(a => a.Account.AccountId == accountId)
                .SelectMany(t => t.Account.Transactions)
                .OrderByDescending(t => t.TransactionId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public ErrorCode CreateAccount(int customerId, int disponentId, string frequency)
        {

            if (customerId == 0 || string.IsNullOrWhiteSpace(frequency)) { return ErrorCode.Error; }

            var newAccount = new Account
            {
                Frequency = frequency,
                Created = DateTime.Now,
                Balance = 0M
            };
            _dbContext.Add(newAccount);
            _dbContext.SaveChanges();

            var newAccountId = newAccount.AccountId;

            var disposition1 = new Disposition
            {
                CustomerId = customerId,
                AccountId = newAccountId,
                Type = "OWNER"
            };
            var disposition2 = new Disposition
            {
                CustomerId = disponentId,
                AccountId = newAccountId,
                Type = "DISPONENT"
            };
            _dbContext.Add(disposition1);
            if (disponentId != 0)
            {
                _dbContext.Add(disposition2);
            }
            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
        public ErrorCode DeleteAccount(int accountId, string type)
        {
            if (type.ToLower() == "owner")
            {
                var dispositions = _dbContext.Dispositions
                       .Include(c => c.Customer)
                       .Include(a => a.Account).ThenInclude(t => t.Transactions)
                       .Include(a => a.Account).ThenInclude(l => l.Loans)
                       .Include(a => a.Account).ThenInclude(p => p.PermenentOrders)
                       .Include(a => a.Account).ThenInclude(d => d.Dispositions).ThenInclude(d => d.Cards)
                       .Where(a => a.AccountId == accountId);
                foreach (var disp in dispositions)
                {
                    if (disp == null) { return ErrorCode.Error; }
                    if (disp!.Account.Balance != 0) { return ErrorCode.Error; }

                    _dbContext.Cards.RemoveRange(disp.Cards);
                    _dbContext.Dispositions.RemoveRange(disp);
                }
            }
            else
            {
                var disposition = _dbContext.Dispositions
                      .Include(c => c.Customer)
                      .Include(a => a.Account)
                      .Where(x => x.Type.ToLower() == "disponent")
                      .FirstOrDefault(a => a.AccountId == accountId);
                if (disposition == null) { return ErrorCode.Error; }
                if (disposition!.Account.Balance != 0) { return ErrorCode.Error; }

                _dbContext.Cards.RemoveRange(disposition.Cards);
                _dbContext.Dispositions.RemoveRange(disposition);
            }
            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
        public ErrorCode Transaction(int accountId, int accountToId, decimal amount)
        {
            if (accountId == accountToId) { return ErrorCode.InvalidAccount; }
            var account = _dbContext.Accounts
                .Include(d => d.Dispositions)
                .ThenInclude(a => a.Customer)
                .First(a => a.AccountId == accountId);

            var accountTo = _dbContext.Accounts
                .Include(d => d.Dispositions)
                .ThenInclude(a => a.Customer)
                .First(a => a.AccountId == accountToId);

            var accountHolder = account.Dispositions
                .Select(c => $"{c.Customer.Givenname} {c.Customer.Surname}").First();
            var accountHolderTo = accountTo.Dispositions
                .Select(c => $"{c.Customer.Givenname} {c.Customer.Surname}").First();

            string transactionMsg;
            string transactionToMsg;

            if (account.Dispositions.Select(c => c.CustomerId).First() ==
                accountTo.Dispositions.Select(c => c.CustomerId).First())
            {
                transactionMsg = $"Remittance to Account {accountTo.AccountId}";
                transactionToMsg = $"Remittance from Account {account.AccountId}";
            }
            else
            {
                transactionMsg = $"Remittance to {accountHolderTo}";
                transactionToMsg = $"Remittance from {accountHolder}";
            }

            if (account.Balance < amount) { return ErrorCode.DeficitBalance; }
            if (amount < 100 || amount > 1000000) { return ErrorCode.AmountOutOfRange; }

            account.Balance -= amount;
            accountTo.Balance += amount;

            _dbContext.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = DateTime.Now,
                Type = "Debit",
                Operation = transactionMsg,
                Amount = -amount,
                Balance = account.Balance,
                Account = accountTo.AccountId.ToString()
            });

            _dbContext.Transactions.Add(new Transaction
            {
                AccountId = accountToId,
                Date = DateTime.Now,
                Type = "Credit",
                Operation = transactionToMsg,
                Amount = amount,
                Balance = accountTo.Balance,
                Account = account.AccountId.ToString()
            });

            _dbContext.SaveChanges();
            return ErrorCode.Success;

        }
        public ErrorCode Deposit(int accountId, decimal amount)
        {
            var account = _dbContext.Accounts.First(a => a.AccountId == accountId);

            if (amount < 100 || amount > 1000000) { return ErrorCode.AmountOutOfRange; }

            account.Balance += amount;
            _dbContext.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = DateTime.Now,
                Type = "Credit",
                Operation = "Credit in Cash",
                Amount = amount,
                Balance = account.Balance
            });
            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
        public ErrorCode Withdraw(int accountId, decimal amount)
        {
            var account = _dbContext.Accounts.First(a => a.AccountId == accountId);

            if (account.Balance < amount) { return ErrorCode.DeficitBalance; }
            if (amount < 100 || amount > 1000000) { return ErrorCode.AmountOutOfRange; }

            account.Balance -= amount;
            _dbContext.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = DateTime.Now,
                Type = "Debit",
                Operation = "Withdrawal in Cash",
                Amount = -amount,
                Balance = account.Balance
            });
            _dbContext.SaveChanges();
            return ErrorCode.Success;
        }
    }
}
