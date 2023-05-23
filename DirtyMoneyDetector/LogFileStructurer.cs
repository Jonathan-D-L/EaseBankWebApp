using Newtonsoft.Json;
using System.Text;
using static DirtyMoneyDetector.LogHandler;

namespace DirtyMoneyDetector
{
    internal class LogFileStructurer
    {
        private const string FileDirectory = "DirtyMoneyDetector_LogFiles";

        internal static void CreateFolderStructure()
        {
            var folderName = FileDirectory;
            var name = DateTime.Now.ToString("yyyy-MM-dd");
            var prefix = "Date-";
            if (!File.Exists($"{folderName}\\{prefix}{name}"))
            {
                if (!Directory.Exists($"{folderName}"))
                {
                    Directory.CreateDirectory($"{folderName}");
                }
                if (!Directory.Exists($"{folderName}\\{prefix}{name}"))
                {
                    Directory.CreateDirectory($"{folderName}\\{prefix}{name}");
                }
            }
        }
        internal static void SerializeObjectsToJsonFileAndTxt(List<TransactionLog> transactionLogs)
        {
            var tempTransactionLog = new List<TransactionLog>();
            var name = DateTime.Now.ToString("yyyy-MM-dd");
            var prefix1 = "Date-";
            var prefix2 = "Log-";
            var subfolder = $"{prefix1}{name}";


            foreach (var transaction in transactionLogs)
            {
                var fileName = $"{prefix2}{name}-country-{transaction.Country}.json"; //change mapping structure based on property
                var filePath = Path.Combine(FileDirectory, subfolder, fileName);

                if (File.Exists($"{FileDirectory}\\{subfolder}\\{fileName}"))
                {
                    var jsonFile = File.ReadAllText($"{FileDirectory}\\{subfolder}\\{fileName}");
                    var jsonDeserialize = JsonConvert.DeserializeObject<IEnumerable<TransactionLog>>(jsonFile);
                    if (jsonDeserialize != null)
                    {
                        tempTransactionLog.Clear();
                        tempTransactionLog.AddRange(jsonDeserialize);
                    }
                }
                if (tempTransactionLog.All(t => t.LogId != transaction.LogId))
                {
                    var jsonString = JsonConvert.SerializeObject(transaction, Formatting.Indented);
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    if (!File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, $"[{jsonString}]");
                    }
                    else
                    {
                        using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
                        if (stream.Length <= 2)
                        {
                            stream.Write(jsonBytes, 0, jsonBytes.Length);
                        }
                        else
                        {
                            stream.Position = stream.Length - 1;
                            stream.Write(Encoding.UTF8.GetBytes($",{jsonString}]"), 0, Encoding.UTF8.GetByteCount($",{jsonString}]"));
                        }
                    }
                }
            }

            var tempTransactionIds = new List<string>();
            foreach (var transaction in transactionLogs)
            {
                var fileName = $"{prefix2}{name}-country-{transaction.Country}.txt"; //change mapping structure based on property
                var filePath = Path.Combine(FileDirectory, subfolder, fileName);

                if (File.Exists($"{FileDirectory}\\{subfolder}\\{fileName}"))
                {
                    var textFile = File.ReadAllText($"{FileDirectory}\\{subfolder}\\{fileName}");
                    var findId = textFile.Split(" ");
                    foreach (var id in findId)
                    {
                        if (id.EndsWith("TYPE:SINGLE"))
                        {
                            tempTransactionIds.Add(id);
                        }
                        if (id.EndsWith("TYPE:TIMESPAN"))
                        {
                            tempTransactionIds.Add(id);
                        }
                    }
                }
                if (tempTransactionIds.All(t => t != transaction.LogId))
                {
                    var textTofile = $"=================================================================\r\n" +
                                     $"Log Date: {transaction.LogDate}\r\n" +
                                     $"Log Id: # {transaction.LogId} #\r\n" +
                                     $"Log Message: {transaction.LogMessage}\r\n" +
                                     $"Account Holder: {transaction.Givenname} {transaction.Surname}\r\n" +
                                     $"Country: {transaction.Country}\r\n";
                    foreach (var ts in transaction.Transactions)
                    {
                        textTofile += $"Transaction Id: {ts.TransactionId} Account: {ts.AccountId} Amount: {ts.Amount} Date: {ts.Date:yy-MM-dd}\r\n";
                    }
                    textTofile += "=================================================================\r\n\r\n";

                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    if (!File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, textTofile);
                    }
                    else
                    {
                        File.AppendAllText(filePath, textTofile);
                    }
                }
            }
            transactionLogs.Clear();
        }
    }
}
