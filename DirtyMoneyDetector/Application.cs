using DataAccessLibrary.Models;
using ServicesLibrary;

namespace DirtyMoneyDetector
{
    internal class Application
    {
     
        internal void Run()
        {
            Console.WriteLine("DirtyMoneyDetector TM");
            Console.WriteLine("Initializing..");
            var dbContext = new BankAppDataContext();
            ICustomerService customerService = new CustomerService(dbContext);
            IAccountService accountService = new AccountService(dbContext);
            LogFileStructurer.CreateFolderStructure();

            Console.WriteLine("Getting the data..");
            var logHandler = new LogHandler(customerService, accountService);
            var transactionLogs = logHandler.GetLogData();
            Console.WriteLine("Finished getting the data..");

            Console.WriteLine("Writing the data..");
            LogFileStructurer.SerializeObjectsToJsonFileAndTxt(transactionLogs);
            Console.WriteLine("Finished writing the data..");
            Console.WriteLine("Exited.");
        }
    }
}
