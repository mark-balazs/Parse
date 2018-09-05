using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary
{
    public class Developer
    {
        public string Name { get; set; }
        public int UserStories { get; set; }
        public int Defects { get; set; }
        public int DefectsDone { get; set; }
        public int DefectsToDo { get; set; }
        public int USDone { get; set; }
        public int USToDo { get; set; }
        public double Effort { get; set; }

        public Developer(string name)
        {
            Name = name;
            UserStories = 0;
            Defects = 0;
            DefectsDone = 0;
            DefectsToDo = 0;
            USDone = 0;
            USToDo = 0;
            Effort = 0;
        }
        public Developer() { }
    }
}
