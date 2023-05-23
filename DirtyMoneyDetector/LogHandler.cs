using ServicesLibrary;

namespace DirtyMoneyDetector
{
    internal class LogHandler
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;

        public LogHandler(ICustomerService customerService, IAccountService accountService)
        {
            _customerService = customerService;
            _accountService = accountService;
        }

        public class TransactionLog
        {
            public DateTime? LogDate { get; set; }
            public string Country { get; set; }
            public string LogId { get; set; }
            public string LogMessage { get; set; }

            public int CustomerId { get; set; }
            public string Givenname { get; set; }
            public string Surname { get; set; }
            public DateTime? Birthday { get; set; }
            public string? NationalId { get; set; }
            public List<TransactionsLog> Transactions { get; set; }

        }

        public class TransactionsLog
        {
            public int TransactionId { get; set; }
            public decimal Amount { get; set; }
            public int AccountId { get; set; }
            public string? AccountToId { get; set; }
            public string Operation { get; set; }
            public DateTime Date { get; set; }
        }

        public List<TransactionLog> TransactionLogs { get; set; } = new();


        public List<TransactionLog> GetLogData()
        {
            var customers = _customerService.GetCustomers();
            var oneTimeMaxVal = 15000M;
            var spanTimeMaxVal = 23000M;
            foreach (var customer in customers)
            {
                //takes last 7 days adjust in service, longer time period will affect performance
                var transactions = _accountService.GetTransactions(customer.CustomerId);

                if (transactions.Any(s => Math.Abs(s.Amount) >= oneTimeMaxVal))
                {

                    foreach (var transaction in transactions)
                    {
                        if (transaction.Amount >= oneTimeMaxVal
                            || ((transaction.Account != null || string.IsNullOrWhiteSpace(transaction.Account))
                            && Math.Abs(transaction.Amount) >= oneTimeMaxVal))
                        {
                            TransactionLogs.Add(new TransactionLog
                            {
                                LogDate = DateTime.Now,
                                Country = customer.Country,
                                LogId = $"{DateTime.Now.Date:yyyyMMdd}{customer.CustomerId}{transaction.TransactionId}TYPE:SINGLE",
                                LogMessage = "Transaction Exceeded Threshold 15 000 SEK",

                                CustomerId = customer.CustomerId,
                                Givenname = customer.Givenname,
                                Surname = customer.Surname,
                                Birthday = customer.Birthday,
                                NationalId = customer.NationalId,
                                Transactions = new List<TransactionsLog>
                                {
                                    new TransactionsLog
                                    {
                                        TransactionId = transaction.TransactionId,
                                        Amount = transaction.Amount,
                                        AccountId = transaction.AccountId,
                                        AccountToId = transaction.Account,
                                        Operation = transaction.Operation,
                                        Date = transaction.Date,
                                    }
                                }
                            });
                        }
                    }
                }

                if (transactions.Where(d => d.Date >= DateTime.Now.AddDays(-3)).Sum(s => Math.Abs(s.Amount)) >= spanTimeMaxVal)
                {
                    var spanTimeTransaction = new TransactionLog
                    {
                        LogDate = DateTime.Now,
                        Country = customer.Country,
                        LogId = $"{DateTime.Now.Date:yyyyMMdd}{customer.CustomerId}{transactions.Last().TransactionId}TYPE:TIMESPAN",
                        LogMessage = "Transactions sum Exceeded Threshold 23 000 SEK within the last 72h",

                        CustomerId = customer.CustomerId,
                        Givenname = customer.Givenname,
                        Surname = customer.Surname,
                        Birthday = customer.Birthday,
                        NationalId = customer.NationalId,
                        Transactions = new List<TransactionsLog>()
                    };
                    foreach (var transaction in transactions)
                    {
                        var transactionItem = new TransactionsLog
                        {
                            TransactionId = transaction.TransactionId,
                            Amount = transaction.Amount,
                            AccountId = transaction.AccountId,
                            AccountToId = transaction.Account,
                            Operation = transaction.Operation,
                            Date = transaction.Date,
                        };
                        spanTimeTransaction.Transactions.Add(transactionItem);
                    }
                    TransactionLogs.Add(spanTimeTransaction);
                }
            }
            return TransactionLogs;
        }
    }
}
