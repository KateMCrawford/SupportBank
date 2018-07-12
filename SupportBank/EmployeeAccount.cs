using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{

    class EmployeeAccount
    {

        private string name;
        private decimal balance;
        private string extraBalance;
        private List<string> transactions = new List<string>();

        public EmployeeAccount(string name)
        {
            this.name = name;
            this.balance = 0;
        }

        public void AddTransaction(Transaction input)
        {
            transactions.Add("On " + input.Date + ", " + input.FromAccount + " gave " + input.ToAccount + " £" + input.Amount + " due to " + input.Narrative + ". ");
            decimal numericalAmount = 0;
            bool isString = false;
            try
            {
                numericalAmount = Convert.ToDecimal(input.Amount);
            }
            catch
            {
                isString = true;
            }
            if (input.FromAccount == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance + numericalAmount;
                }
                else
                    extraBalance += ", plus " + input.Amount;
            }
            if (input.ToAccount == this.name)
            {
                if (!isString)
                {
                    this.balance = this.balance - numericalAmount;
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
            }
        }

        public void WriteBalance()
        {
            Console.WriteLine(this.name + " has balance " + this.balance + this.extraBalance + ". ");
        }
    }


}
