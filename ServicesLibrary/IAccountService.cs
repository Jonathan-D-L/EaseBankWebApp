using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;
using UtilityLibrary;

namespace ServicesLibrary
{
    public interface IAccountService
    {
        int GetAccountsCount(int customerId);
        decimal GetAccountsSum(int customerId);
        int GetLoansCount(int customerId);
        decimal GetLoansSum(int customerId);
        int GetPaymentsCount(int customerId);
        int GetTransactionsCount(int customerId);
        decimal GetAccountTotalValuePerCountry(string country);

        void UpdateAccount(Account account);
        Account GetAccount(int accountId);

        PagedResult<Account> GetAccounts(int customerId, int pageNum);
        PagedResult<Loan> GetLoans(int customerId, string sortColumn, string sortOrder, int pageNum, string q);
        PagedResult<PermenentOrder> GetPermanentOrders(int customerId, string sortColumn, string sortOrder, int pageNum, string q);
        List<Transaction> GetTransactions(int customerId);
        PagedResult<Transaction> GetTransactions(int customerId, string sortColumn, string sortOrder, int pageNum, string q);
        PagedResult<Transaction> GetTransactionsForAccount(int customerId, int accountId, int page);
        Task<List<Transaction>> GetTransactionsAsync(int customerId, int accountId, int limit, int offset);

        ErrorCode CreateAccount(int customerId, int disponentId, string frequency);
        ErrorCode DeleteAccount(int accountId, string type);
        ErrorCode Withdraw(int accountId, decimal amount);
        ErrorCode Deposit(int accountId, decimal amount);
        ErrorCode Transaction(int accountId, int accountToId, decimal amount);
    }
}
