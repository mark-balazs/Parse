using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
using static System.IO.Directory;
using static System.IO.File;


namespace MainLibrary
{
     public partial class Program
    {
        public class MainClass
        {
            protected string CsvFile { get; set; }
            protected string LogPath { get; set; }
            protected string Mode { get; set; }
            protected string[] Args { get; set; }
            protected int ArgNum { get; set; }
            protected MainObject MainObject { get; set; }


            public MainClass(string[] arguments)
            {
                MainObject = new MainObject();
                CsvFile = null;
                LogPath = null;
                Mode = null;
                ArgNum = 0;
                Args = arguments;
            }

            public void Parse()
            {
                GetArgsNumber();
                SetFilePaths();
                GetMode();
                if (Mode == "q")
                    return;
                if (File.Exists(LogPath))
                    Deserialize();
                GetVersion();
                if (ArgNum == 2)
                    ProcessCsv(ReadAllText(CsvFile));
                if (ArgNum < 2)
                    GetDataFromKeyboard();
                WriteAllText(LogPath, JsonConvert.SerializeObject(MainObject));
                WriteLine("File saved at " + LogPath);
            }

            protected void GetArgsNumber()
            {
                foreach (string str in Args)
                    ArgNum++;
            }

            protected void SetFilePaths()
            {
                switch (ArgNum)
                {
                    case 0:
                        LogPath = GetCurrentDirectory() + "\\changelog.json";
                        break;
                    case 1:
                        if (Args[0].Contains(".json"))
                            LogPath = Args[0];
                        else
                        {
                            CsvFile = Args[0];
                            LogPath = GetCurrentDirectory() + "\\changelog.json";
                            ArgNum++;
                        }
                        break;
                    case 2:
                        LogPath = Args[1];
                        CsvFile = Args[0];
                        break;
                }
            }

            protected void GetMode()
            {
                bool answer = false;
                while (!answer)
                {
                    WriteLine("Modes:\n[ENTER] - Updating version automatically\n[1] - Adding new version from keyboard\n[q] - Exit\nEnter Mode:");
                    Mode = ReadLine();
                    if (Mode == "1" || Mode == "")
                        answer = true;
                    else if (Mode == "q")
                        return;
                }
            }

            protected void Deserialize()
            {
                string data = ReadAllText(LogPath);
                MainObject = JsonConvert.DeserializeObject<MainObject>(data);
            }

            protected void GetVersion()
            {
                if (Mode == "1")
                {
                    GetVersionFromKeyboard();
                }
                else
                {
                    if (!File.Exists(LogPath))
                        MainObject.AddVersion("1.0.0");
                    else
                    {
                        if (CountDots() == 2)
                        {
                            string[] vSplit = MainObject.Versions[0].VersionId.Split('.');
                            int minor = Int32.Parse(vSplit[2]);
                            minor++;
                            vSplit[2] = minor.ToString();
                            string version = vSplit[0] + '.' + vSplit[1] + '.' + vSplit[2];
                            MainObject.AddVersion(version);
                        }
                        else if (CountDots() == 1)
                            MainObject.Versions[0].VersionId += ".1";
                        else if (CountDots() == 0)
                            MainObject.Versions[0].VersionId += ".0.1";
                    }
                }
            }

            protected void GetVersionFromKeyboard()
            {
                WriteLine("Enter Version:");
                MainObject.AddVersion(ReadLine());
                if (CountDots() == 1)
                {
                    MainObject.Versions[0].VersionId += ".0";
                }
                else if (CountDots() == 0)
                {
                    MainObject.Versions[0].VersionId += ".0.0";
                }
            }

            protected int CountDots()
            {
                int count = 0;
                foreach (char c in MainObject.Versions[0].VersionId)
                {
                    if (c == '.')
                        count++;
                }
                return count;
            }

            protected void ProcessCsv(string data)
            {
                    BuildEntities(GetTokens(DeleteCommas(data)));
            }

            protected string DeleteCommas(string data)
            {
                var data2 = data.ToCharArray();
                bool str = false;
                int n = 0;
                foreach (char a in data2)
                {
                    if (a == '"')
                    {
                        str = !str;
                    }
                    if (a == ',' && str)
                    {
                        data2[n] = ';';
                    }
                    n++;
                }
                return new string(data2);
            }

            protected List<string> GetTokens(string data)
            {
                List<string> tokens = new List<string>();
                foreach (string l in data.Split(','))
                {
                    tokens.Add(l);
                }
                if (tokens.Count() < 84)
                {
                    WriteLine("ERROR: Too few data in file \"" + CsvFile + "\".");
                    Environment.Exit(0);
                    ArgNum--;
                }
                return tokens;
            }

            protected void BuildEntities(List<string> tokens)
            {
                int index = 1;
                while (index + 43 < tokens.Count())
                {
                    index += 42;
                    Entity ent = new Entity(tokens[index + 2], tokens[index + 1]);
                    if (tokens[index] == "Bug" && (tokens[index + 36] == "Done" || tokens[index + 36] == "Rejected" || tokens[index + 36] == "Integration Testing Passed"))
                    {
                        MainObject.Versions[0].Changelog.Defects.Add(ent);
                    }
                    else if (tokens[index] == "UserStory" && (tokens[index + 36] == "Done" || tokens[index + 36] == "Rejected" || tokens[index + 36] == "Integration Testing Passed"))
                    {
                        MainObject.Versions[0].Changelog.UserStories.Add(ent);
                    }
                }
            }

            protected void GetDataFromKeyboard()
            {
                string decision = "";
                while(true)
                {
                    WriteLine("Would you like to add data manually? (ENTER/n)");
                    decision = ReadLine();
                    if (decision == "" || decision == "n")
                        break;
                }
                if (decision == "n")
                    return;
                string lastDecision = "";
                while (true)
                {
                    decision = GetAnswer(lastDecision);
                    if (decision == "q")
                        break;
                    if (decision == "")
                        decision = lastDecision;
                    if (decision == "1")
                    {
                        MainObject.Versions[0].Changelog.Defects.Add(GetEntityInfo());
                        WriteLine("[ENTER] - Add another bug.");
                    }
                    else if (decision == "2")
                    {
                        MainObject.Versions[0].Changelog.UserStories.Add(GetEntityInfo());
                        WriteLine("[ENTER] - Add another user story.");
                    }
                    lastDecision = decision;
                }
            }

            protected string GetAnswer(string lastDecision)
            {
                string decision = "";
                while (true)
                {
                    if (lastDecision != "1")
                        WriteLine("[1] - Bug");
                    if (lastDecision != "2")
                        WriteLine("[2] - User story");
                    WriteLine("[q] - Finish adding data from keyboard");
                    decision = ReadLine();
                    if (decision == "1")
                        break;
                    if (decision == "2")
                        break;
                    if (decision == "" && lastDecision != "")
                        break;
                    if (decision == "q")
                        break;
                    WriteLine(decision + "a");
                }
                return decision;
            }
            protected Entity GetEntityInfo()
            {
                Entity entity = new Entity();
                WriteLine("Id:");
                entity.Id = ReadLine();
                WriteLine("Description:");
                entity.Description = ReadLine();
                WriteLine("URL:");
                entity.URL = ReadLine();
                WriteLine("RequestId:");
                entity.RequestId = ReadLine();
                WriteLine("RequestUrl:");
                entity.RequestUrl = ReadLine();
                return entity;
            }
            public string Csv()
            {
                return CsvFile;
            }
        }
    }
}