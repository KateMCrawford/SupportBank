using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupportBank
{
    class WriteFile
    {
        public void FileWriter(string input, List<Transaction> transactionList)
        {
            string filePath = input.Substring(12);
            if (filePath.EndsWith(".csv"))
            {
                File.WriteAllText(filePath, "");
                for (int i = 0; i < transactionList.Count; i++)
                {
                    File.AppendAllText(filePath, transactionList[i].Date + "," + transactionList[i].FromAccount + "," + transactionList[i].ToAccount + "," + transactionList[i].Narrative + "," + transactionList[i].Amount + Environment.NewLine);
                }
            }
            //if (filePath.EndsWith(".csv"))
            //{
            //    File.WriteAllText(filePath, "");
            //    string printJson = "";
            //    for (int i = 0; i < transactionList.Count; i++)
            //    {
            //        FileStream textWriter = new FileStream(filePath, FileMode.Append);
            //        JsonSerializer serializer = new JsonSerializer();
            //        serializer.Serialize(textWriter, transactionList[i]);
            //        File.AppendAllText(filePath, textWriter + Environment.NewLine);
            //    }
            //}

        }
    }
}
