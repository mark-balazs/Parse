using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;



namespace Parse
{
    class Program
    {
        public class Entity
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string URL { get; set; }
            public string RequestId { get; set; }
            public string RequestUrl { get; set; }


            public Entity(string a, string b)
            {
                Description = a;
                Id = b;
            }
            public Entity() { }
        }

        public class Changelog
        {
            public List<Entity> Defects { get; set; }
            public List<Entity> UserStories { get; set; }
            public Changelog()
            {
                Defects = new List<Entity>();
                UserStories = new List<Entity>();
            }

        }

        public class Version
        {
            public string VersionId { get; set; }
            public Changelog Changelog { get; set; }

            public Version()
            {
                Changelog = new Changelog();
            }
            public Version(string rhs)
            {
                Changelog = new Changelog();
                VersionId = rhs;
            }
        }

        public class MainObject
        {
            public List<Version> Versions { get; set; }

            public MainObject()
            {
                Versions = new List<Version>();
            }

            public void AddVersion(string foo)
            {
                Version rhs = new Version(foo);
                Versions.Insert(0, rhs);
            }
        }


        static void Main(string[] args)
        {
            string file = null;
            string logpath = null;
            int argnum = 0;
            foreach(string a in args)
            {
                argnum++;
            }
            switch (argnum)
            {
                case 0:
                    logpath = Directory.GetCurrentDirectory() + "\\changelog.json";
                    break;
                case 1:
                    if (args[0].Contains(".json"))
                        logpath = args[0];
                    else
                    {
                        file = args[0];
                        logpath = Directory.GetCurrentDirectory() + "\\changelog.json";
                        argnum++;
                    }
                    break;
                case 2:
                    logpath = args[0];
                    file = args[1];
                    break;
            }
            bool answer = false;
            string mode = null;
            while (!answer)
            {
                Console.WriteLine("Modes:\n[ENTER] - Updating version automatically\n[1] - Adding new version from keyboard\n[q] - Exit\nEnter mode:");
                mode = Console.ReadLine();
                if (mode == "1" || mode == "")
                    answer = true;
                if (mode == "q")
                    return;
            }
            MainObject mainObject = new MainObject();
                


            if (File.Exists(logpath))
            {
                string jdata = File.ReadAllText(logpath);
                mainObject = JsonConvert.DeserializeObject<MainObject>(jdata);
            }

            if (mode == "1")
            {
                Console.WriteLine("Enter Version:");
                mainObject.AddVersion(Console.ReadLine());
                int count = 0;
                foreach (char c in mainObject.Versions[0].VersionId)
                {
                    if (c == '.')
                        count++;
                }
                if (count == 1)
                {
                    mainObject.Versions[0].VersionId += ".0";
                }
                else if (count == 0)
                {
                    mainObject.Versions[0].VersionId += ".0.0";
                }
            }
            else
            {
                if (!File.Exists(logpath))
                {
                    mainObject.AddVersion("1.0.0");
                }
                else
                {
                    int count = 0;
                    foreach (char c in mainObject.Versions[0].VersionId)
                    {
                        if (c == '.')
                            count++;
                    }
                    if (count == 2)
                    {
                        string[] vSplit = mainObject.Versions[0].VersionId.Split('.');
                        int minor = Int32.Parse(vSplit[2]);
                        minor++;
                        vSplit[2] = minor.ToString();
                        string version = vSplit[0] + '.' + vSplit[1] + '.' + vSplit[2];
                        mainObject.AddVersion(version);
                    }
                    else if (count == 1)
                    {
                        mainObject.Versions[0].VersionId += ".1";
                    }
                    else if (count == 0)
                    {
                        mainObject.Versions[0].VersionId += ".0.1";
                    }
                }
            }
            if(argnum>=2)
            {
                string dat = File.ReadAllText(file);
                var data2 = dat.ToCharArray();


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

                dat = new string(data2);

                List<string> tokens = new List<string>();


                foreach (string l in dat.Split(','))
                {
                    tokens.Add(l);
                }

                if (tokens.Count() < 84)
                {
                    Console.WriteLine("ERROR: Too few data in file \"" + file + "\".");
                    argnum--;
                }

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
            if (argnum < 2)
            {
                bool fromkeyboard = false;
                Console.WriteLine("Would you like to add data manually? (ENTER/n)");
                var decision = Console.ReadLine();
                string ld = null;
                fromkeyboard = (decision == "");
                decision = null;
                while (fromkeyboard)
                {
                    Entity ent = new Entity();
                    answer = false;
                    while (!answer)
                    {
                        if (ld != "1")
                            Console.WriteLine("[1] - Bug");
                        if (ld != "2")
                            Console.WriteLine("[2] - User story");
                        Console.WriteLine("[q] - Finish adding data from keyboard");
                        decision = Console.ReadLine();
                        answer = (decision == "1" || decision == "2" || decision == "q" || (decision == "" && ld != null));
                        if (decision == "q")
                            break;
                    }
                    if (decision == "q")
                        break;
                    Console.WriteLine("Id:");
                    ent.Id = Console.ReadLine();
                    Console.WriteLine("Description:");
                    ent.Description = Console.ReadLine();
                    Console.WriteLine("URL:");
                    ent.URL = Console.ReadLine();
                    Console.WriteLine("RequestId:");
                    ent.RequestId = Console.ReadLine();
                    Console.WriteLine("RequestUrl:");
                    ent.RequestUrl = Console.ReadLine();
                    if(decision=="")
                    {
                        if(ld == "1")
                        {
                            mainObject.Versions[0].Changelog.Defects.Add(ent);
                            Console.WriteLine("[ENTER] - Add another bug.");
                        }
                        else if(ld == "2")
                        {
                            mainObject.Versions[0].Changelog.UserStories.Add(ent);
                            Console.WriteLine("Add another user story.");
                        }
                    }
                    else if (decision == "1")
                    {
                        mainObject.Versions[0].Changelog.Defects.Add(ent);
                        ld = decision;
                        Console.WriteLine("[ENTER] - Add another bug.");
                    }
                    else if (decision == "2")
                    {
                        mainObject.Versions[0].Changelog.UserStories.Add(ent);
                        ld = decision;
                        Console.WriteLine("[ENTER] - Add another user story.");
                    }
                }
            }


            var data = JsonConvert.SerializeObject(mainObject);

            //logpath = Directory.GetCurrentDirectory() + "\\changelog.json";
            File.WriteAllText(logpath, data);

            Console.WriteLine("File saved at " + logpath);

        }

    }
}