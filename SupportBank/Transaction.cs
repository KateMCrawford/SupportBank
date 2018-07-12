using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Transaction
    {
        public string Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public string Amount { get; set; }

        public Transaction(string Date, string FromAccount, string ToAccount, string Narrative, string Amount)
        {
            this.Date = Date;
            this.FromAccount = FromAccount;
            this.ToAccount = ToAccount;
            this.Narrative = Narrative;
            this.Amount = Amount;
        }
    }

}
