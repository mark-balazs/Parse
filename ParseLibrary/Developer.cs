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
        public List<Ticket> UserStoriesContainer { get; set; }
        public List<Ticket> DefectsContainer { get; set; }
        public List<Ticket> DefectsDoneContainer { get; set; }
        public List<Ticket> DefectsToDoContainer { get; set; }
        public List<Ticket> USDoneContainer { get; set; }
        public List<Ticket> USToDoContainer { get; set; }

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
