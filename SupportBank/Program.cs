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

        public EmployeeAccount(string name)
        {
            this.name = name;
            this.balance = 0;
        }

        public void ChangeBalance(decimal amount)
        {
            this.balance = this.balance + amount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new StreamReader(@"C:\Users\KMC\Downloads\SupportBank-master\Transactions2014.csv"))
            {
                List<string> fromList = new List<string>();
                List<string> toList = new List<string>();
                List<decimal> amountList = new List<decimal>();

                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    fromList.Add(values[1]);
                    toList.Add(values[2]);
                    amountList.Add(Convert.ToDecimal(values[4]));
                }

                IEnumerable<string> distinctNames = fromList.Concat(toList).ToList().Distinct();

                List<EmployeeAccount> accountList = new List<EmployeeAccount>();

                foreach(string name in distinctNames)
                {
                    accountList.Add(new EmployeeAccount(name));
                }

                Console.ReadLine();
            }
        }
    }
}
