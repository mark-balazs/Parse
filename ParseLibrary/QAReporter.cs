using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Microsoft.Office.Interop.Excel;
using static System.IO.File;
using static System.IO.Directory;

namespace MainLibrary
{
    public class QAReporter : Reporter
    {
        public QAContainer Testers { get; set; }
        public QAReporter(string[] arguments)
            : base(arguments)
        {
            Testers = new QAContainer();
        }
        public new void MakeReport()
        {
            SetFilePaths();
            tokens = GetTokens(DeleteCommas(ReadAllText(CsvFile)));
            BuildEntities();
            WriteToSheet();
        }
        protected new void SetFilePaths()
        {
            CsvFile = Args[0];
            //CsvFile = @"D:\\Sprint Planning.csv";
            xlPath = GetCurrentDirectory() + "qareport.xlsx";
            //jsPath = GetCurrentDirectory() + "testers.json";
        }
        protected new void BuildEntities()
        {
            int index = 1;
            while (index + 43 < tokens.Count())
            {
                index += 42;
                if (tokens[index + 37] != null)
                {
                    AnalyzeNames(tokens[index + 37].Split(';'), index);
                }
            }
        }
        protected new void AnalyzeNames(string[] names, int index)
        {
            foreach (string fullname in names)
            {
                if (fullname.Contains("QA Engineer"))
                {
                    var name = fullname.Split(')')[1];
                    name = FixTypos(name);
                    if (!Testers.Contains(name))
                        Testers.Container.Add(new Tester(name));
                    AddTicket(name, index);
                    if (tokens[index] == "Bug")
                        AddBug(name, index);
                    if (tokens[index] == "UserStory")
                        AddUS(name, index);
                }
            }
        }
        protected void AddTicket(string name, int index)
        {
            Ticket ticket = new Ticket();
            ticket.Id = tokens[index + 1];
            ticket.Title = tokens[index + 2];
            ticket.State = tokens[index + 36];
            ticket.Severity = tokens[index + 39];
            Testers.Container[Testers.Index(name)].Tickets.Add(ticket);
        }
        protected new void AddBug(string name, int index)
        {
            Testers.Container[Testers.Index(name)].DefectsSum++;
            if (Testers.Container[Testers.Index(name)].Defects.ContainsKey(tokens[index + 39]))
                Testers.Container[Testers.Index(name)].Defects[tokens[index + 39]]++;
            else
                Testers.Container[Testers.Index(name)].Defects.Add(tokens[index + 39], 1);
        }
        protected new void AddUS(string name, int index)
        {
            Testers.Container[Testers.Index(name)].UserStoriesSum++;
            if (Testers.Container[Testers.Index(name)].UserStories.ContainsKey(tokens[index + 39]))
                Testers.Container[Testers.Index(name)].UserStories[tokens[index + 39]]++;
            else
                Testers.Container[Testers.Index(name)].UserStories.Add(tokens[index + 39], 1);

        }
        protected new void WriteToSheet()
        {
            xlWorkSheet.Name = "Sum";
            xlWorkSheet.Cells[1, 1] = "Tester Name";
            xlWorkSheet.Cells[1, 2] = "Sum of Defect tickets released";
            xlWorkSheet.Cells[1, 3] = "Severity: 'Blocking'";
            xlWorkSheet.Cells[1, 4] = "Severity: 'Critical'";
            xlWorkSheet.Cells[1, 5] = "Severity: 'Normal'";
            xlWorkSheet.Cells[1, 6] = "Severity: 'Small'";
            xlWorkSheet.Cells[1, 7] = "Severity: 'Enhancement'";
            xlWorkSheet.Cells[1, 8] = "Sum of User Story tickets released";
            foreach (Tester t in Testers.Container)
            {
                WriteLine();
            }
            for (int i = 2; i < Testers.Container.Count + 2; i++)
            {
                xlWorkSheet.Cells[i, 1] = Testers.Container[i - 2].Name;
                xlWorkSheet.Cells[i, 2] = Testers.Container[i - 2].DefectsSum;
                if (Testers.Container[i - 2].Defects.ContainsKey("Blocking"))
                    xlWorkSheet.Cells[i, 3] = Testers.Container[i - 2].Defects["Blocking"];
                else
                    xlWorkSheet.Cells[i, 3] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Critical"))
                    xlWorkSheet.Cells[i, 4] = Testers.Container[i - 2].Defects["Critical"];
                else
                    xlWorkSheet.Cells[i, 4] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Normal"))
                    xlWorkSheet.Cells[i, 5] = Testers.Container[i - 2].Defects["Normal"];
                else
                    xlWorkSheet.Cells[i, 5] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Small"))
                    xlWorkSheet.Cells[i, 6] = Testers.Container[i - 2].Defects["Small"];
                else
                    xlWorkSheet.Cells[i, 6] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Enhancement"))
                    xlWorkSheet.Cells[i, 7] = Testers.Container[i - 2].Defects["Enhancement"];
                else
                    xlWorkSheet.Cells[i, 7] = 0;

                xlWorkSheet.Cells[i, 8] = Testers.Container[i - 2].UserStoriesSum;
                /*if (Testers.Container[i - 2].UserStories.ContainsKey("Blocking"))
                    xlWorkSheet.Cells[i, 9] = Testers.Container[i - 2].UserStories["Blocking"];
                else
                    xlWorkSheet.Cells[i, 9] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Critical"))
                    xlWorkSheet.Cells[i, 10] = Testers.Container[i - 2].UserStories["Critical"];
                else
                    xlWorkSheet.Cells[i, 10] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Normal"))
                    xlWorkSheet.Cells[i, 11] = Testers.Container[i - 2].UserStories["Normal"];
                else
                    xlWorkSheet.Cells[i, 11] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Small"))
                    xlWorkSheet.Cells[i, 12] = Testers.Container[i - 2].UserStories["Small"];
                else
                    xlWorkSheet.Cells[i, 12] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Enhancement"))
                    xlWorkSheet.Cells[i, 13] = Testers.Container[i - 2].UserStories["Enhancement"];
                else
                    xlWorkSheet.Cells[i, 13] = 0;*/
            }
            ListAllTestersInDifferentSheets();
        }
        protected void ListAllTestersInDifferentSheets()
        {
            for (int i = 2; i < Testers.Container.Count + 2; i++)
            {
                xlWorkSheet = xlWorkBook.Worksheets.Add();
                xlWorkSheet.Name = Testers.Container[i - 2].Name;
                xlWorkSheet.Cells[1, 1] = "Id";
                xlWorkSheet.Cells[1, 2] = "Title";
                xlWorkSheet.Cells[1, 3] = "Severity";
                xlWorkSheet.Cells[1, 4] = "State";
                for (int j = 2; j < Testers.Container[i - 2].Tickets.Count + 2; j++)
                {
                    xlWorkSheet.Cells[j, 1] = Testers.Container[i - 2].Tickets[j - 2].Id;
                    xlWorkSheet.Cells[j, 2] = Testers.Container[i - 2].Tickets[j - 2].Title;
                    xlWorkSheet.Cells[j, 3] = Testers.Container[i - 2].Tickets[j - 2].Severity;
                    xlWorkSheet.Cells[j, 4] = Testers.Container[i - 2].Tickets[j - 2].State;
                }
            }
            xlWorkSheet = xlWorkBook.Worksheets.get_Item(xlWorkBook.Worksheets.Count);
            xlWorkSheet.Select();
            xlWorkBook.SaveAs(xlPath);
            WriteLine("Report saved as " + xlPath);
            xlWorkBook.Close(0);
            xlApp.Quit();
        }
    }
}
