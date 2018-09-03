using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
using static System.IO.Directory;
using static System.IO.File;


namespace Parse
{
    internal partial class Program
    {
        public class MainClass
        {
            private string CsvFile { get; set; }
            private string LogPath { get; set; }
            private string Mode { get; set; }
            private string[] Args { get; set; }
            private int ArgNum { get; set; }
            private MainObject mainObject { get; set; }


            public MainClass(string[] arguments)
            {
                mainObject = new MainObject();
                CsvFile = null;
                LogPath = null;
                Mode = null;
                ArgNum = 0;
                Args = arguments;

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
                WriteAllText(LogPath, JsonConvert.SerializeObject(mainObject));
                WriteLine("File saved at " + LogPath);
            }

            private void GetArgsNumber()
            {
                foreach (string str in Args)
                    ArgNum++;
            }

            private void SetFilePaths()
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
                        LogPath = Args[0];
                        CsvFile = Args[1];
                        break;
                }
            }

            private void GetMode()
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

            private void Deserialize()
            {
                string data = ReadAllText(LogPath);
                mainObject = JsonConvert.DeserializeObject<MainObject>(data);
            }

            private void GetVersion()
            {
                if (Mode == "1")
                {
                    GetVersionFromKeyboard();
                }
                else
                {
                    if (!File.Exists(LogPath))
                        mainObject.AddVersion("1.0.0");
                    else
                    {
                        if (CountDots() == 2)
                        {
                            string[] vSplit = mainObject.Versions[0].VersionId.Split('.');
                            int minor = Int32.Parse(vSplit[2]);
                            minor++;
                            vSplit[2] = minor.ToString();
                            string version = vSplit[0] + '.' + vSplit[1] + '.' + vSplit[2];
                            mainObject.AddVersion(version);
                        }
                        else if (CountDots() == 1)
                            mainObject.Versions[0].VersionId += ".1";
                        else if (CountDots() == 0)
                            mainObject.Versions[0].VersionId += ".0.1";
                    }
                }
            }

            private void GetVersionFromKeyboard()
            {
                WriteLine("Enter Version:");
                mainObject.AddVersion(ReadLine());
                if (CountDots() == 1)
                {
                    mainObject.Versions[0].VersionId += ".0";
                }
                else if (CountDots() == 0)
                {
                    mainObject.Versions[0].VersionId += ".0.0";
                }
            }

            private int CountDots()
            {
                int count = 0;
                foreach (char c in mainObject.Versions[0].VersionId)
                {
                    if (c == '.')
                        count++;
                }
                return count;
            }

            private void ProcessCsv(string data)
            {
                    BuildEntities(GetTokens(DeleteCommas(data)));
            }

            private string DeleteCommas(string data)
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
                        data2[n] = ' ';
                    }
                    n++;
                }
                return new string(data2);
            }

            private List<string> GetTokens(string data)
            {
                List<string> tokens = new List<string>();
                foreach (string l in data.Split(','))
                {
                    tokens.Add(l);
                }
                if (tokens.Count() < 84)
                {
                    WriteLine("ERROR: Too few data in file \"" + CsvFile + "\".");
                    ArgNum--;
                }
                return tokens;
            }

            private void BuildEntities(List<string> tokens)
            {
                int index = 1;
                while (index + 43 < tokens.Count())
                {
                    index += 42;
                    Entity ent = new Entity(tokens[index + 2], tokens[index + 1]);
                    if (tokens[index] == "Bug" && (tokens[index + 36] == "Done" || tokens[index + 36] == "Rejected" || tokens[index + 36] == "Integration Testing Passed"))
                    {
                        mainObject.Versions[0].Changelog.Defects.Add(ent);
                    }
                    else if (tokens[index] == "UserStory" && (tokens[index + 36] == "Done" || tokens[index + 36] == "Rejected" || tokens[index + 36] == "Integration Testing Passed"))
                    {
                        mainObject.Versions[0].Changelog.UserStories.Add(ent);
                    }
                }
            }

            private void GetDataFromKeyboard()
            {
                bool readFromKeyboard = false;
                WriteLine("Would you like to add data manually? (ENTER/n)");
                string decision = ReadLine();
                string lastDecision = null;
                readFromKeyboard = (decision == "");
                while (readFromKeyboard)
                {
                    GetAnswer(lastDecision);
                    if (decision == "q")
                        break;
                    if (decision == "")
                        decision = lastDecision;
                    if (decision == "1")
                    {
                        mainObject.Versions[0].Changelog.Defects.Add(GetEntityInfo());
                        WriteLine("[ENTER] - Add another bug.");
                    }
                    else if (decision == "2")
                    {
                        mainObject.Versions[0].Changelog.UserStories.Add(GetEntityInfo());
                        WriteLine("[ENTER] - Add another user story.");
                    }
                    lastDecision = decision;
                }
            }

            private string GetAnswer(string lastDecision)
            {
                string decision = null;
                bool answer = false;
                while (!answer)
                {
                    if (lastDecision != "1")
                        WriteLine("[1] - Bug");
                    if (lastDecision != "2")
                        WriteLine("[2] - User story");
                    WriteLine("[q] - Finish adding data from keyboard");
                    decision = ReadLine();
                    answer = (decision == "1" || decision == "2" || decision == "q" || (decision == "" && lastDecision != null));
                    if (decision == "q")
                        break;
                }
                return decision;
            }
            private Entity GetEntityInfo()
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
        }
    }
}