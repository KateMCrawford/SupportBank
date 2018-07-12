using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog.Config;
using NLog.Targets;
using NLog;
using Newtonsoft.Json;
using Newtonsoft;
using System.Xml;


namespace SupportBank
{

    class EmployeeAccount
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private string name;
        private decimal balance;
        private string extraBalance;
        private List<string> transactions = new List<string>();

        public EmployeeAccount(string name)
        {
            this.name = name;
            this.balance = 0;
            logger.Info("Account created for name " + name);
        }

        public void AddTransaction(Transaction input)
        {
            transactions.Add("On " + input.Date + ", " + input.FromAccount + " gave " + input.ToAccount + " £" + input.Amount + " due to " + input.Narrative + ". ");
            logger.Info("Transaction added");
            decimal numericalAmount = 0;
            bool isString = false;
            try
            {
                numericalAmount = Convert.ToDecimal(input.Amount);
            }
            catch
            {
                isString = true;
                logger.Info("String given as monetary input");
            }
            if (input.FromAccount == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance + numericalAmount;
                    logger.Info("Increasing " + this.name + " balance by " + input.Amount);
                }
                else
                    extraBalance += ", plus " + input.Amount;
            }
            if (input.ToAccount == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance - numericalAmount;
                    logger.Info("Decreasing " + this.name + " balance by " + input.Amount);
                }
                else
                    extraBalance += ", minus " + input.Amount;
            }
        }

        public void ListTransactions()
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine(transactions[i]);
                logger.Info("Printing transaction line " + Convert.ToString(i));
            }
        }

        public void WriteBalance()
        {
            Console.WriteLine(this.name + " has balance " + this.balance + this.extraBalance + ". ");
            logger.Info("Writing balance for " + this.name);
        }
    }

    class Transaction
    {
        public string Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public string Amount { get; set; }

        public Transaction(string Date, string FromAccout, string ToAccount, string Narrative, string Amount)
        {
            this.Date = Date;
            this.FromAccount = FromAccount;
            this.ToAccount = ToAccount;
            this.Narrative = Narrative;
            this.Amount = Amount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Start logging

            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Users\KMC\Work\Training\SupportBank\Logs\SupportBankLogs.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            ILogger logger = LogManager.GetCurrentClassLogger();

            //Wait for file to import

            bool notInput = true;
            string fileLocation = "";
            while (notInput)
            {
                string locationInput = Console.ReadLine();
                if (locationInput.ToLower().StartsWith("import file "))
                {
                    fileLocation = locationInput.Substring(12);
                    if (File.Exists((fileLocation)) && ((fileLocation.EndsWith(".csv")) || (fileLocation.EndsWith(".json")) || (fileLocation.EndsWith(".xml"))))
                        notInput = false;
                    else
                        Console.WriteLine("That file does not exist or is not a readable format");
                }
                else
                    Console.WriteLine("Please input file location to be imported");

            }

            List<Transaction> transactionList = new List<Transaction>();

            logger.Info("List initialised");

            //Load file

            if (fileLocation.EndsWith(".csv"))
            {
                logger.Info("File ending is .csv");
                using (var reader = new StreamReader(fileLocation))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        transactionList.Add(new Transaction(values[0], values[1], values[2], values[3], values[4]));

                    }


                }
            }
            else if (fileLocation.EndsWith(".json"))
            {
                logger.Info("File ending is .json");
                string readData = File.ReadAllText(fileLocation);
                transactionList = JsonConvert.DeserializeObject<List<Transaction>>(readData);

            }
            else if (fileLocation.EndsWith(".xml"))
            {
                string date;
                string from;
                string to;
                string narrative;
                string amount;

                XmlReader reader = XmlReader.Create(fileLocation);
                while (reader.Read())
                {
                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "SupportTransaction")))
                    {
                        reader.Read();
                    }
                    date = Convert.ToString(reader.ReadContentAsDateTime());

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Description")))
                    {
                        reader.Read();
                    }
                    narrative = reader.ReadContentAsString();

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Value")))
                    {
                        reader.Read();
                    }
                    amount = reader.ReadContentAsString();

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "From")))
                    {
                        reader.Read();
                    }
                    from = reader.ReadContentAsString();

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "To")))
                    {
                        reader.Read();
                    }
                    to = reader.ReadContentAsString();

                    transactionList.Add(new Transaction(date, from, to, narrative, amount));
                }
            }
            else
            {
                Console.WriteLine("Error: something is very broken. This error should not be possible");
                logger.Info("Very broken, line 129");
            }

            Console.WriteLine("File loaded");

            List<string> allNames = new List<string>();

            for (int i = 0; i < transactionList.Count; i++)
            {
                allNames.Add(transactionList[i].FromAccount);
                allNames.Add(transactionList[i].ToAccount);
            }

            IEnumerable<string> distinctNames = allNames.Distinct();

            Dictionary<string, EmployeeAccount> accountList = new Dictionary<string, EmployeeAccount>();

            logger.Info("Dictionary created");

            foreach (string name in distinctNames)
            {
                try
                {
                    accountList.Add(name, new EmployeeAccount(name));
                }
                catch
                {
                    logger.Info("Transaction was input without name");
                    Console.WriteLine("Error: make sure all transactions have associated names");
                }
            }

            for (int i = 0; i < transactionList.Count; i++)
            {
                accountList[transactionList[i].FromAccount].AddTransaction(transactionList[i]);
                accountList[transactionList[i].ToAccount].AddTransaction(transactionList[i]);
            }

            logger.Info("Transactions loaded into accounts.");

            bool exit = false;

            while (!exit)
            {

                bool foundName = false;

                string input = Console.ReadLine();
                if (input.ToLower() == "list all")
                    foreach (string name in distinctNames)
                    {
                        accountList[name].WriteBalance();
                    }
                else if (input.StartsWith("list "))
                {
                    string inputName = input.Substring(5);
                    foreach (string name in distinctNames)
                    {
                        if (name.ToLower() == inputName.ToLower())
                        {
                            accountList[name].ListTransactions();
                            foundName = true;
                        }
                    }
                    if (!foundName)
                        Console.WriteLine("This name is not recognised.");
                }
                else if (input.ToLower() == "exit")
                {
                    exit = true;
                }
                else
                    Console.WriteLine("This command was not recognised.");

            }
        }


    }
}
