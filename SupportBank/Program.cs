using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SupportBank
{
    class EmployeeAccount
    {
        private string name;
        private decimal balance;
        private List<string> transactions = new List<string>();

        public EmployeeAccount(string name)
        {
            this.name = name;
            this.balance = 0;
        }
        
        public void AddTransaction(string date, string nameFrom, string nameTo, string narrative, decimal amount)
        {
            transactions.Add("On " + date + ", " + nameFrom + " gave " + nameTo + " £" + Convert.ToString(amount) + " due to " + narrative + ". ");
            if (nameFrom == this.name)
                this.balance = this.balance + amount;
            if (nameTo == this.name)
                this.balance = this.balance - amount;
        }

        public void ListTransactions()
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine(transactions[i]);
            }
        }

        public void WriteBalance()
        {
            Console.WriteLine(this.name + " has balance " + this.balance + ". ");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new StreamReader(@"C:\Users\KMC\Downloads\SupportBank-master\Transactions2014.csv"))
            {
                List<string> dateList = new List<string>();
                List<string> fromList = new List<string>();
                List<string> toList = new List<string>();
                List<string> narrativeList = new List<string>();
                List<decimal> amountList = new List<decimal>();

                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    dateList.Add(values[0]);
                    fromList.Add(values[1]);
                    toList.Add(values[2]);
                    narrativeList.Add(values[3]);
                    amountList.Add(Convert.ToDecimal(values[4]));
                }

                IEnumerable<string> distinctNames = fromList.Concat(toList).ToList().Distinct();

                Dictionary<string, EmployeeAccount> accountList = new Dictionary<string, EmployeeAccount>();

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
            }
        }
    }
}
