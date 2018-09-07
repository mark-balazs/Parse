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
                    if (tokens[index] == "Bug")
                        AddBug(name, index);
                    if (tokens[index] == "UserStory")
                        AddUS(name, index);
                }
            }
        }
        protected new void AddBug(string name, int index)
        {
            Testers.Container[Testers.Index(name)].DefectsSum++;
            if (Testers.Container[Testers.Index(name)].Defects.ContainsKey(tokens[index + 36]))
                Testers.Container[Testers.Index(name)].Defects[tokens[index + 36]]++;
            else
                Testers.Container[Testers.Index(name)].Defects.Add(tokens[index + 36], 1);
        }

        protected new void AddUS(string name, int index)
        {
            Testers.Container[Testers.Index(name)].UserStoriesSum++;
            if (Testers.Container[Testers.Index(name)].UserStories.ContainsKey(tokens[index + 36]))
                Testers.Container[Testers.Index(name)].UserStories[tokens[index + 36]]++;
            else
                Testers.Container[Testers.Index(name)].UserStories.Add(tokens[index + 36], 1);

        }

        protected new void WriteToSheet()
        {
            xlWorkSheet.Cells[1, 1] = "Tester name";
            xlWorkSheet.Cells[1, 2] = "Defects Assigned";
            xlWorkSheet.Cells[1, 3] = "Defects Done";
            xlWorkSheet.Cells[1, 4] = "Defects Open";
            xlWorkSheet.Cells[1, 5] = "Defects Rejected";
            xlWorkSheet.Cells[1, 6] = "Defects Integration Testing Passed";
            xlWorkSheet.Cells[1, 7] = "User Stories Assigned";
            xlWorkSheet.Cells[1, 8] = "User Stories Done";
            xlWorkSheet.Cells[1, 9] = "User Stories Open";
            xlWorkSheet.Cells[1, 10] = "User Stories Rejected";
            xlWorkSheet.Cells[1, 11] = "User Stories Integration Testing Passed";
            foreach(Tester t in Testers.Container)
            {
                WriteLine();
            }
            for (int i = 2; i < Testers.Container.Count + 2; i++)
            {
                xlWorkSheet.Cells[i, 1] = Testers.Container[i - 2].Name;
                xlWorkSheet.Cells[i, 2] = Testers.Container[i - 2].DefectsSum;
                if (Testers.Container[i - 2].Defects.ContainsKey("Done"))
                    xlWorkSheet.Cells[i, 3] = Testers.Container[i - 2].Defects["Done"];
                else
                    xlWorkSheet.Cells[i, 3] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Open"))
                    xlWorkSheet.Cells[i, 4] = Testers.Container[i - 2].Defects["Open"];
                else
                    xlWorkSheet.Cells[i, 4] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Rejected"))
                    xlWorkSheet.Cells[i, 5] = Testers.Container[i - 2].Defects["Rejected"];
                else
                    xlWorkSheet.Cells[i, 5] = 0;
                if (Testers.Container[i - 2].Defects.ContainsKey("Integration Testing Passed"))
                    xlWorkSheet.Cells[i, 6] = Testers.Container[i - 2].Defects["Integration Testing Passed"];
                else
                    xlWorkSheet.Cells[i, 6] = 0;

                xlWorkSheet.Cells[i, 7] = Testers.Container[i - 2].UserStoriesSum;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Done"))
                    xlWorkSheet.Cells[i, 8] = Testers.Container[i - 2].UserStories["Done"];
                else
                    xlWorkSheet.Cells[i, 8] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Open"))
                    xlWorkSheet.Cells[i, 9] = Testers.Container[i - 2].UserStories["Open"];
                else
                    xlWorkSheet.Cells[i, 9] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Rejected"))
                    xlWorkSheet.Cells[i, 10] = Testers.Container[i - 2].UserStories["Rejected"];
                else
                    xlWorkSheet.Cells[i, 10] = 0;
                if (Testers.Container[i - 2].UserStories.ContainsKey("Integration Testing Passed"))
                    xlWorkSheet.Cells[i, 11] = Testers.Container[i - 2].UserStories["Integration Testing Passed"];
                else
                    xlWorkSheet.Cells[i, 11] = 0;
            }
            xlWorkBook.SaveAs(xlPath);
            WriteLine("Report saved as " + xlPath);

            xlWorkBook.Close(0);
            xlApp.Quit();
        }
    }
}
