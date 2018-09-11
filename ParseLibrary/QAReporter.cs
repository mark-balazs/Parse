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
            ticket.EventType = tokens[index];
            ticket.Priority = tokens[index + 35];
            ticket.Title = tokens[index + 2];
            ticket.State = tokens[index + 36];
            ticket.Severity = tokens[index + 39];
            Testers.Container[Testers.Index(name)].Tickets.Add(ticket);
        }
        protected new void AddBug(string name, int index)
        {
            Testers.Container[Testers.Index(name)].DefectsSum++;
            if (Testers.Container[Testers.Index(name)].Defects.ContainsKey(tokens[index + 39]))//severity
                Testers.Container[Testers.Index(name)].Defects[tokens[index + 39]]++;
            else
                Testers.Container[Testers.Index(name)].Defects.Add(tokens[index + 39], 1);
            if (Testers.Container[Testers.Index(name)].Defects.ContainsKey(tokens[index + 35]))//priority
                Testers.Container[Testers.Index(name)].Defects[tokens[index + 35]]++;
            else
                Testers.Container[Testers.Index(name)].Defects.Add(tokens[index + 35], 1);
            Testers.Container[Testers.Index(name)].BugEffort += Double.Parse(tokens[index + 14]);
        }
        protected new void AddUS(string name, int index)
        {
            Testers.Container[Testers.Index(name)].UserStoriesSum++;
            if (Testers.Container[Testers.Index(name)].UserStories.ContainsKey(tokens[index + 35]))
                Testers.Container[Testers.Index(name)].UserStories[tokens[index + 35]]++;
            else
                Testers.Container[Testers.Index(name)].UserStories.Add(tokens[index + 35], 1);
            Testers.Container[Testers.Index(name)].USEffort += Double.Parse(tokens[index + 14]);
        }
        protected new void WriteToSheet()
        {
            xlWorkSheet.Name = "Summary";
            xlWorkSheet.Cells[1, 1] = "Defects";
            xlWorkSheet.Cells[1, 2] = "Tester Name";
            xlWorkSheet.Cells[1, 3] = "Sum of Defect tickets assigned";
            xlWorkSheet.Cells[1, 8] = "Severity: 'Enhancement'";
            xlWorkSheet.Cells[1, 7] = "Severity: 'Small'";
            xlWorkSheet.Cells[1, 6] = "Severity: 'Normal'";
            xlWorkSheet.Cells[1, 5] = "Severity: 'Critical'";
            xlWorkSheet.Cells[1, 4] = "Severity: 'Blocking'";
            xlWorkSheet.Cells[1, 9] = "Effort";
            int i;
            for (i = 2; i < Testers.Container.Count + 2; i++)
            {
                xlWorkSheet.Cells[i, 2] = Testers.Container[i - 2].Name;
                xlWorkSheet.Cells[i, 3] = Testers.Container[i - 2].DefectsSum;
                if (Testers.Container[i - 2].Defects.ContainsKey("Enhancement"))
                    xlWorkSheet.Cells[i, 4] = Testers.Container[i - 2].Defects["Enhancement"];
                else
                    xlWorkSheet.Cells[i, 4] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Small"))
                    xlWorkSheet.Cells[i, 5] = Testers.Container[i - 2].Defects["Small"];
                else
                    xlWorkSheet.Cells[i, 5] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Normal"))
                    xlWorkSheet.Cells[i, 6] = Testers.Container[i - 2].Defects["Normal"];
                else
                    xlWorkSheet.Cells[i, 6] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Critical"))
                    xlWorkSheet.Cells[i, 7] = Testers.Container[i - 2].Defects["Critical"];
                else
                    xlWorkSheet.Cells[i, 7] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Blocking"))
                    xlWorkSheet.Cells[i, 8] = Testers.Container[i - 2].Defects["Blocking"];
                else
                    xlWorkSheet.Cells[i, 8] = 0;
                xlWorkSheet.Cells[i, 9] = Testers.Container[i - 2].BugEffort;
            }
            i += 2;
            xlWorkSheet.Cells[i, 1] = "User stories";
            xlWorkSheet.Cells[i, 2] = "Tester name";
            xlWorkSheet.Cells[i, 3] = "Sum of User Story tickets assigned";
            xlWorkSheet.Cells[i, 4] = "Priority: 'Low'";
            xlWorkSheet.Cells[i, 5] = "Priority: 'Medium Low'";
            xlWorkSheet.Cells[i, 6] = "Priority: 'Medium'";
            xlWorkSheet.Cells[i, 7] = "Priority: 'Medium High'";
            xlWorkSheet.Cells[i, 8] = "Priority: 'High'";
            xlWorkSheet.Cells[i, 9] = "Priority: 'Fix If Time'";
            xlWorkSheet.Cells[i, 10] = "Priority: 'Fix ASAP'";
            xlWorkSheet.Cells[i, 11] = "Effort";
            for (i = i + 1; i < Testers.Container.Count * 2 + 5; i++)
            {
                xlWorkSheet.Cells[i, 2] = Testers.Container[i - 5 - Testers.Container.Count].Name;
                xlWorkSheet.Cells[i, 3] = Testers.Container[i - 5 - Testers.Container.Count].UserStoriesSum;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Low"))
                    xlWorkSheet.Cells[i, 4] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Low"];
                else
                    xlWorkSheet.Cells[i, 4] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Medium Low"))
                    xlWorkSheet.Cells[i, 5] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Medium Low"];
                else
                    xlWorkSheet.Cells[i, 5] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Medium"))
                    xlWorkSheet.Cells[i, 6] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Medium"];
                else
                    xlWorkSheet.Cells[i, 6] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Medium High"))
                    xlWorkSheet.Cells[i, 7] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Medium High"];
                else
                    xlWorkSheet.Cells[i, 7] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("High"))
                    xlWorkSheet.Cells[i, 8] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["High"];
                else
                    xlWorkSheet.Cells[i, 8] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Fix If Time"))
                    xlWorkSheet.Cells[i, 9] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Fix If Time"];
                else
                    xlWorkSheet.Cells[i, 9] = 0;
                if (Testers.Container[i - 5 - Testers.Container.Count].UserStories.ContainsKey("Fix ASAP"))
                    xlWorkSheet.Cells[i, 10] = Testers.Container[i - 5 - Testers.Container.Count].UserStories["Fix ASAP"];
                else
                    xlWorkSheet.Cells[i, 10] = 0;
                xlWorkSheet.Cells[i, 11] = Testers.Container[i - 5 - Testers.Container.Count].USEffort;
            }
            ListAllTestersInDifferentSheets();
        }
        protected void ListAllTestersInDifferentSheets()
        {
            for (int i = 2; i < Testers.Container.Count + 2; i++)
            {
                xlWorkSheet = xlWorkBook.Worksheets.Add();
                xlWorkSheet.Name = Testers.Container[i - 2].Name;
                xlWorkSheet.Cells[1, 1] = "Event Type";
                xlWorkSheet.Cells[1, 2] = "Id";
                xlWorkSheet.Cells[1, 3] = "Title";
                xlWorkSheet.Cells[1, 4] = "Severity";
                xlWorkSheet.Cells[1, 5] = "Priority";
                xlWorkSheet.Cells[1, 6] = "State";
                int j,k=0;
                for (j = 2; j < Testers.Container[i - 2].Tickets.Count + 2; j++)
                {
                    if (Testers.Container[i - 2].Tickets[j - 2].EventType == "Bug")
                    {
                        xlWorkSheet.Cells[j - k, 1] = Testers.Container[i - 2].Tickets[j - 2].EventType;
                        xlWorkSheet.Cells[j - k, 2] = Testers.Container[i - 2].Tickets[j - 2].Id;
                        xlWorkSheet.Cells[j - k, 3] = Testers.Container[i - 2].Tickets[j - 2].Title;
                        xlWorkSheet.Cells[j - k, 4] = Testers.Container[i - 2].Tickets[j - 2].Severity;
                        xlWorkSheet.Cells[j - k, 5] = Testers.Container[i - 2].Tickets[j - 2].Priority;
                        xlWorkSheet.Cells[j - k, 6] = Testers.Container[i - 2].Tickets[j - 2].State;
                    }
                    else
                        k++;
                }
                k = 0;
                for(;j<Testers.Container[i-2].Tickets.Count*2 +2;j++)
                {
                    if (Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].EventType == "UserStory")
                    {
                        xlWorkSheet.Cells[j - k, 1] = Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].EventType;
                        xlWorkSheet.Cells[j - k, 2] = Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].Id;
                        xlWorkSheet.Cells[j - k, 3] = Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].Title;
                        xlWorkSheet.Cells[j - k, 5] = Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].Priority;
                        xlWorkSheet.Cells[j - k, 6] = Testers.Container[i - 2].Tickets[j - Testers.Container[i - 2].Tickets.Count - 2].State;
                    }
                    else
                        k++;
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
