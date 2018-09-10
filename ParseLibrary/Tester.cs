using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary
{
    public class Tester
    {
        public string Name { get; set; }
        public int  DefectsSum { get; set; }
        public int UserStoriesSum { get; set; }
        public Dictionary<string,int> Defects { get; set; }
        public Dictionary<string,int> UserStories { get; set; }
        public List<Ticket> Tickets { get; set; }

        public Tester(string name)
        {
            Name = name;
            Defects = new Dictionary<string, int>();
            UserStories = new Dictionary<string, int>();
            Tickets = new List<Ticket>();
            DefectsSum = 0;
            UserStoriesSum = 0;
;        }
    }
}
