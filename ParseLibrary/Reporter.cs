using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using static System.Console;
using static System.IO.File;
using static System.Environment;
using static System.IO.Directory;
using System.Runtime.InteropServices;
using static MainLibrary.Program;
using Newtonsoft.Json;


namespace MainLibrary
{
    public class Reporter : MainClass
    {
        protected string xlPath;
        protected string jsPath;
        protected Application xlApp { get; set; }
        protected Workbook xlWorkBook { get; set; }
        protected Worksheet xlWorkSheet { get; set; }
        protected DevContainer Developers { get; set; }
        protected List<string> tokens { get; set; }


        public Reporter(string[] arguments)
            : base(arguments)
        {
            tokens = new List<string>();
            Developers = new DevContainer();
            xlApp = new Application();
            if (xlApp == null)
            {
                WriteLine("ERROR: Excel is not installed correctly.");
            }
            else
            {
                xlWorkBook = xlApp.Workbooks.Add(1);
                xlWorkSheet = xlWorkBook.Worksheets.get_Item(1);
            }
            GetArgsNumber();
            if (ArgNum < 1)
            {
                WriteLine("Arguments do not meet the requirements for creating a report.");
                Exit(0);
            }
        }

        public void MakeReport()
        {
            SetFilePaths();
            tokens = GetTokens(DeleteCommas(ReadAllText(CsvFile)));
            BuildEntities();
            WriteToSheet();
        }
        protected new void SetFilePaths()
        {
            CsvFile = Args[0];
            //CsvFile = @"D:\Informatics\Intern projects\Parse\DevReport\sample.csv";
            xlPath = GetCurrentDirectory() + "devreport.xlsx";
            jsPath = @"D:/Informatics/Javascript/Development2/Development-visualization/developers.json";
        }

        protected void BuildEntities()
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

        protected void AnalyzeNames(string[] names, int index)
        {
            foreach (string fullname in names)
            {
                if (fullname.Contains("Developer"))
                {
                    var name = fullname.Split(')')[1];
                    name = FixTypos(name);
                    if (!Developers.Contains(name))
                        Developers.AddDeveloper(name);
                    if (tokens[index] == "Bug")
                        AddBug(name, index);
                    if (tokens[index] == "UserStory")
                        AddUS(name, index);
                }
            }
        }

        protected void AddBug(string name, int index)
        {
            Developers.Container[Developers.Index(name)].Defects++;
            if (tokens[index + 36] == "Rejected" || tokens[index + 36] == "Done" || tokens[index + 36] == "Integration Testing Passed")
            {
                Developers.Container[Developers.Index(name)].DefectsDone++;
                Ticket ticket = new Ticket();
                ticket.Id = tokens[index + 1];
                ticket.EventType = tokens[index];
                ticket.Priority = tokens[index + 35];
                ticket.Title = tokens[index + 2];
                ticket.State = tokens[index + 36];
                ticket.Severity = tokens[index + 39];
                Developers.Container[Developers.Index(name)].DefectsContainer.Add(ticket);
                Developers.Container[Developers.Index(name)].DefectsDoneContainer.Add(ticket);
            }
            else
            {
                Developers.Container[Developers.Index(name)].DefectsToDo++;
                Ticket ticket = new Ticket();
                ticket.Id = tokens[index + 1];
                ticket.EventType = tokens[index];
                ticket.Priority = tokens[index + 35];
                ticket.Title = tokens[index + 2];
                ticket.State = tokens[index + 36];
                ticket.Severity = tokens[index + 39];
                Developers.Container[Developers.Index(name)].DefectsToDoContainer.Add(ticket);
                Developers.Container[Developers.Index(name)].DefectsContainer.Add(ticket);
            }
        }

        protected void AddUS(string name, int index)
        {
            Developers.Container[Developers.Index(name)].UserStories++;
            if (tokens[index + 36] == "Rejected" || tokens[index + 36] == "Done" || tokens[index + 36] == "Integration Testing Passed")
            {
                Developers.Container[Developers.Index(name)].USDone++;
                Ticket ticket = new Ticket();
                ticket.Id = tokens[index + 1];
                ticket.EventType = tokens[index];
                ticket.Priority = tokens[index + 35];
                ticket.Title = tokens[index + 2];
                ticket.State = tokens[index + 36];
                ticket.Severity = tokens[index + 39];
                Developers.Container[Developers.Index(name)].USDoneContainer.Add(ticket);
                Developers.Container[Developers.Index(name)].UserStoriesContainer.Add(ticket);
            }
            else
            {
                Developers.Container[Developers.Index(name)].USToDo++;
                Ticket ticket = new Ticket();
                ticket.Id = tokens[index + 1];
                ticket.EventType = tokens[index];
                ticket.Priority = tokens[index + 35];
                ticket.Title = tokens[index + 2];
                ticket.State = tokens[index + 36];
                ticket.Severity = tokens[index + 39];
                Developers.Container[Developers.Index(name)].USToDoContainer.Add(ticket);
                Developers.Container[Developers.Index(name)].UserStoriesContainer.Add(ticket);
            }
            Developers.Container[Developers.Index(name)].Effort += Double.Parse(tokens[index + 15]);
        }

        protected void WriteToSheet()
        {
            xlWorkSheet.Cells[1, 1] = "Developer name";
            xlWorkSheet.Cells[1, 2] = "All user stories assigned";
            xlWorkSheet.Cells[1, 3] = "User stories done";
            xlWorkSheet.Cells[1, 4] = "User stories to do";
            xlWorkSheet.Cells[1, 5] = "Effort done";
            xlWorkSheet.Cells[1, 6] = "All defects assigned";
            xlWorkSheet.Cells[1, 7] = "Defects done";
            xlWorkSheet.Cells[1, 8] = "Defects to do";
            int index = 2;
            foreach(Developer dev in Developers.Container)
            {
                xlWorkSheet.Cells[index, 1] = dev.Name;
                xlWorkSheet.Cells[index, 2] = dev.UserStories;
                xlWorkSheet.Cells[index, 3] = dev.USDone;
                xlWorkSheet.Cells[index, 4] = dev.USToDo;
                xlWorkSheet.Cells[index, 5] = dev.Effort;
                xlWorkSheet.Cells[index, 6] = dev.Defects;
                xlWorkSheet.Cells[index, 7] = dev.DefectsDone;
                xlWorkSheet.Cells[index, 8] = dev.DefectsToDo;
                index++;
            }
            xlWorkBook.SaveAs(xlPath);
            WriteLine("Report saved as " + xlPath);

            Marshal.FinalReleaseComObject(xlWorkSheet);
            xlWorkBook.Close(0);
            Marshal.FinalReleaseComObject(xlWorkBook);
            xlApp.Quit();
            Marshal.FinalReleaseComObject(xlApp);
        }

        protected string FixTypos(string name)
        {
            string[] split = name.Split('"');
            foreach(string str in split)
            {
                if(str.Length > 0)
                {
                    name = str;
                    break;
                }
            }
            return name;
        }

        public void Serializer()
        {
            string serializedData = JsonConvert.SerializeObject(Developers.Container);
            WriteAllText(jsPath, serializedData);
        }
    }
}
