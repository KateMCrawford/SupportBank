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

            LoadFile fileLoader = new LoadFile();
            transactionList = fileLoader.LoadFileMethod(fileLocation);

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
                else if (input.ToLower().StartsWith("export file "))
                {
                    WriteFile fileWriter = new WriteFile();
                    fileWriter.FileWriter(input, transactionList);
                }
                else
                    Console.WriteLine("This command was not recognised.");

            }
        }


    }
}
