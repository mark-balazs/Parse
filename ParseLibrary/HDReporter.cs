using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.IO.File;
using static System.Environment;
using static System.Console;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace MainLibrary
{
    public class HDReporter
    {

        public Application xlApplication { get; set; }
        public Workbook xlWorkbook { get; set; }
        public Worksheet xlWorksheet { get; set; }

        public string[] CsvPaths { get; set; }
        public string SummaryPath { get; set; }
        public string SaveFolder { get; set; }
        public List<string> Tokens { get; set; }
        public int ArgumentsNumber { get; set; }
        public Dictionary<string, List<Conversation>> Conversations { get; set; }
        public List<Bill> Bills { get; set; }


        public HDReporter(string[] csvpaths, string summarypath)
        {
            ArgumentsNumber = 0;
            Tokens = new List<string>();
            Conversations = new Dictionary<string, List<Conversation>>();
            Bills = new List<Bill>();
            CsvPaths = csvpaths;
            SummaryPath = summarypath;
            GetArgsNumber();
        }
        public void MakeReport()
        {
            SetFilePaths();
            GetTokens();
            GetBillingInfo();
            BuildLists();
            SetBillings();
            WriteToCsv();
            CreateBillingSummary();
        }
        private void BuildLists()
        {
            int index = 0;
            while (index + 9 < Tokens.Count)
            {
                index += 8;
                AnalizeToken(index);
            }
        }
        private void SetBillings()
        {
            foreach (Bill bill in Bills)
            {
                if (GetNumberFromString(bill.BundleMinutes) < bill.UsedMinutes)
                {
                    bill.Price = (bill.UsedMinutes - GetNumberFromString(bill.BundleMinutes)) * 0.03;
                }
            }
        }

        protected bool IsInternational(string number)
        {
            return number.StartsWith("11");
        }

        protected bool IsTollFree(string number)
        {
            return number.StartsWith("1800") || number.StartsWith("800") || number.StartsWith("844") || number.StartsWith("1844") || number.StartsWith("855") || number.StartsWith("1855") || number.StartsWith("866") || number.StartsWith("1866") || number.StartsWith("877") || number.StartsWith("1877") || number.StartsWith("888") || number.StartsWith("1888");
        }

        protected int GetIndexByTenant(string tenant)
        {
            for (int i = 0; i < Bills.Count; i++)
            {
                if (Bills[i].Tenant.Equals(tenant))
                    return i;
            }
            return -1;
        }

        private void AnalizeToken(int index)
        {
            Conversation conversation = ExtractConversation(index);
            if (!Conversations.ContainsKey(Tokens[index]))
            {
                Conversations.Add(Tokens[index], new List<Conversation>());
            }
            Conversations[Tokens[index]].Add(conversation);
            if (IsTollFree(conversation.To))
            {
                if (GetIndexByTenant(conversation.Tenant) != -1)
                    Bills[GetIndexByTenant(conversation.Tenant)].AddTollFreeMinutes(conversation.Billing);
            }
            else
            {
                if (IsInternational(conversation.To))
                {
                    if (GetIndexByTenant(conversation.Tenant) != -1)
                        Bills[GetIndexByTenant(conversation.Tenant)].AddInterMinutes(conversation.Billing);
                }
                if (GetIndexByTenant(conversation.Tenant) != -1)
                    Bills[GetIndexByTenant(conversation.Tenant)].AddMinutes(conversation.Billing);
            }
        }
        protected Conversation ExtractConversation(int index)
        {
            Conversation con = new Conversation();
            con.Tenant = Tokens[index];
            con.From = Tokens[index + 1];
            con.To = Tokens[index + 2];
            con.Date = Tokens[index + 3];
            con.Duration = Tokens[index + 4];
            con.Billing = Tokens[index + 5];
            con.Cost = Tokens[index + 6];
            con.Status = Tokens[index + 7];
            return con;
        }
        protected void SetFilePaths()
        {
            xlApplication = new Application();
            xlWorkbook = xlApplication.Workbooks.Open(SummaryPath);
            xlWorksheet = xlWorkbook.Sheets[1];
        }
        protected void GetArgsNumber()
        {
            foreach (string str in CsvPaths)
                ArgumentsNumber++;
            if (ArgumentsNumber < 1)
                Exit(0);
        }
        protected void GetTokens()
        {
            foreach (string CsvPath in CsvPaths)
            {
                foreach (string str in ReadAllText(CsvPath).Split('\t'))
                {
                    string[] str2 = str.Split('\n');
                    foreach (string s in str2)
                    {
                        Tokens.Add(s);
                    }
                }
                if (Tokens.Count < 16)
                {
                    WriteLine("Not enough data in the file:" + CsvPath);
                    Exit(0);
                }
            }
        }
        protected void GetBillingInfo()
        {
            for (int i = 2; i <= xlWorksheet.UsedRange.Rows.Count; i++)
            {
                if (xlWorksheet.Rows[i].Hidden)
                    continue;
                if (xlWorksheet.Cells[i, 1].Value2 != null)
                {
                    Bill bill = new Bill();
                    bill.Tenant = xlWorksheet.Cells[i, 1].Value2.ToString();
                    if (xlWorksheet.Cells[i, 2].Value2 != null)
                        bill.CustomerName = xlWorksheet.Cells[i, 2].Value2;
                    if (xlWorksheet.Cells[i, 3].Value2 != null)
                        bill.ContractNumber = xlWorksheet.Cells[i, 3].Value2.ToString();
                    if (xlWorksheet.Cells[i, 4].Value2 != null)
                        bill.BundleMinutes = xlWorksheet.Cells[i, 4].Value2.ToString();
                    Bills.Add(bill);
                }
            }
            Marshal.FinalReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.FinalReleaseComObject(xlWorkbook);
            xlApplication.Quit();
            Marshal.FinalReleaseComObject(xlApplication);
        }
        protected void WriteToCsv()
        {
            foreach (KeyValuePair<string, List<Conversation>> pair in Conversations)
            {
                string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", Tokens[0], Tokens[1], Tokens[2], Tokens[3], Tokens[4], Tokens[5], Tokens[6], Tokens[7]);
                foreach (Conversation conv in pair.Value)
                {
                    data += string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", conv.Tenant, conv.From, conv.To, conv.Date, conv.Duration, conv.Billing, conv.Cost, conv.Status);
                }
                if (GetIndexByTenant(pair.Key) != -1)
                {
                    WriteAllText(SaveFolder + "/" + Bills[GetIndexByTenant(pair.Key)].CustomerName + ".csv", data);
                }
                else
                {
                    WriteAllText(SaveFolder + "/Tenant " + pair.Key + ".csv", data);
                }
            }
        }
        protected void CreateBillingSummary()
        {
            xlApplication = new Application();
            xlWorkbook = xlApplication.Workbooks.Add();
            xlWorksheet = xlWorkbook.Sheets.Add();

            xlWorksheet.Cells[1, 1] = "Tenant";
            xlWorksheet.Cells[1, 2] = "Customer Name";
            xlWorksheet.Cells[1, 3] = "Contract Number";
            xlWorksheet.Cells[1, 4] = "Bundle Minutes";
            xlWorksheet.Cells[1, 5] = "Used Minutes";
            xlWorksheet.Cells[1, 6] = "Price";
            xlWorksheet.Cells[1, 7] = "International minutes";
            xlWorksheet.Cells[1, 8] = "Price";
            xlWorksheet.Cells[1, 9] = "Toll Free minutes";
            xlWorksheet.Cells[1, 10] = "Price";
            xlWorksheet.Cells[1, 11] = "TOTAL DUE";

            for (int i = 2; i < Bills.Count + 2; i++)
            {
                xlWorksheet.Cells[i, 1] = Bills[i - 2].Tenant;
                xlWorksheet.Cells[i, 2] = Bills[i - 2].CustomerName;
                xlWorksheet.Cells[i, 3] = Bills[i - 2].ContractNumber;
                xlWorksheet.Cells[i, 4] = Bills[i - 2].BundleMinutes;
                xlWorksheet.Cells[i, 5] = Bills[i - 2].UsedMinutes;
                xlWorksheet.Cells[i, 6] = Bills[i - 2].Price;
                xlWorksheet.Cells[i, 7] = Bills[i - 2].InternMinutes;
                xlWorksheet.Cells[i, 8] = Bills[i - 2].InterPrice;
                xlWorksheet.Cells[i, 9] = Bills[i - 2].TollFreeMinutes;
                xlWorksheet.Cells[i, 10] = Bills[i - 2].TFPrice;
                xlWorksheet.Cells[i, 11] = Bills[i - 2].TotalDue;
            }
            xlWorkbook.SaveAs(SaveFolder + "\\BillingSummary.xlsx");
            Marshal.FinalReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.FinalReleaseComObject(xlWorkbook);
            xlApplication.Quit();
            Marshal.FinalReleaseComObject(xlApplication);
        }
        protected double GetNumberFromString(string str)
        {
            double n = 0;
            foreach (char c in str)
            {
                if (c >= '0' && c <= '9')
                {
                    n *= 10;
                    n += c - '0';
                }
                else
                {
                    break;
                }
            }
            return n;
        }
    }
}
