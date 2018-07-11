using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog.Config;
using NLog.Targets;
using NLog;
using Json;

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
        
        public void AddTransaction(string date, string nameFrom, string nameTo, string narrative, string amount)
        {
            transactions.Add("On " + date + ", " + nameFrom + " gave " + nameTo + " £" + amount + " due to " + narrative + ". ");
            logger.Info("Transaction added");
            decimal numericalAmount = 0;
            bool isString = false;
            try
            {
                numericalAmount = Convert.ToDecimal(amount);
            }
            catch
            {
                isString = true;
                logger.Info("String given as monetary input");
            }
            if (nameFrom == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance + numericalAmount;
                    logger.Info("Increasing " + this.name + " balance by " + amount);
                }
                else
                    extraBalance += ", plus " + amount;
            }
            if (nameTo == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance - numericalAmount;
                    logger.Info("Decreasing " + this.name + " balance by " + amount);
                }
                else
                    extraBalance += ", minus " + amount;
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
        private string Date;
        private string FromAccount;
        private string ToAccount;
        private string Narrative;
        private string Amount;
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
                    if (File.Exists((fileLocation)) && ((fileLocation.EndsWith(".csv")) || (fileLocation.EndsWith(".json"))))
                        notInput = false;
                    else
                        Console.WriteLine("That file does not exist or is not a readable format");
                }
                else
                    Console.WriteLine("Please input file location to be imported");

            }

            List<string> dateList = new List<string>();
            List<string> fromList = new List<string>();
            List<string> toList = new List<string>();
            List<string> narrativeList = new List<string>();
            List<string> amountList = new List<string>();

            logger.Info("Lists initialised");

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

                        dateList.Add(values[0]);
                        fromList.Add(values[1]);
                        toList.Add(values[2]);
                        narrativeList.Add(values[3]);
                        amountList.Add(values[4]);
                    }


                }
            }
            else if (fileLocation.EndsWith(".json"))
            {
                logger.Info("File ending is .json");
                JsonConverter.DeserializeObject<List<Transaction>>(fileLocation);
            }
            else
            {
                Console.WriteLine("Error: something is very broken. This error should not be possible");
                logger.Info("Very broken, line 129");
            }
                    IEnumerable<string> distinctNames = fromList.Concat(toList).ToList().Distinct();

                    Dictionary<string, EmployeeAccount> accountList = new Dictionary<string, EmployeeAccount>();

                    logger.Info("Dictionary created");

                    foreach (string name in distinctNames)
                    {
                        accountList.Add(name, new EmployeeAccount(name));
                    }

                    for (int i = 0; i < dateList.Count; i++)
                    {
                        accountList[fromList[i]].AddTransaction(dateList[i], fromList[i], toList[i], narrativeList[i], amountList[i]);
                        accountList[toList[i]].AddTransaction(dateList[i], fromList[i], toList[i], narrativeList[i], amountList[i]);
                    }

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
                    else
                        Console.WriteLine("This command was not recognised.");


                    Console.ReadLine();
                
            }
            
        
    }
}
