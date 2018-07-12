using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace SupportBank
{
    class LoadFile
    {
        public List<Transaction> LoadFileMethod(string fileLocation)
        {
            List<Transaction> transactionList = new List<Transaction>;
            if (fileLocation.EndsWith(".csv"))
            {
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
                string readData = File.ReadAllText(fileLocation);
                transactionList = JsonConvert.DeserializeObject<List<Transaction>>(readData);

            }
            else if (fileLocation.EndsWith(".xml"))
            {
                string date = "";
                string from = "";
                string to = "";
                string narrative = "";
                string amount = "";

                XmlReader reader = XmlReader.Create(fileLocation);
                while (reader.Read())
                {
                    bool keepTransaction = true;
                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "SupportTransaction")) && (reader.NodeType != XmlNodeType.None))
                    {
                        reader.Read();
                    }
                    if (reader.NodeType != XmlNodeType.None)
                        date = Convert.ToString(reader.GetAttribute(0));

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Description")) && (reader.NodeType != XmlNodeType.None))
                    {
                        reader.Read();
                    }
                    if (reader.NodeType != XmlNodeType.None)
                        narrative = reader.ReadElementContentAsString();

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Value")) && (reader.NodeType != XmlNodeType.None))
                    {
                        reader.Read();
                    }
                    if (reader.NodeType != XmlNodeType.None)
                        amount = reader.ReadElementContentAsString();

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "From")) && (reader.NodeType != XmlNodeType.None))
                    {
                        reader.Read();
                    }
                    if (reader.NodeType != XmlNodeType.None)
                        from = reader.ReadElementContentAsString();
                    if (from == null)
                        keepTransaction = false;

                    while (!((reader.NodeType == XmlNodeType.Element) && (reader.Name == "To")) && (reader.NodeType != XmlNodeType.None))
                    {
                        reader.Read();
                    }
                    if (reader.NodeType != XmlNodeType.None)
                        to = reader.ReadElementContentAsString();
                    if (to == null)
                        keepTransaction = false;

                    if (keepTransaction)
                        transactionList.Add(new Transaction(date, from, to, narrative, amount));
                }
            }
            else
            {
                Console.WriteLine("Error: something is very broken. This error should not be possible");
            }
            return transactionList


        }
    }
}
