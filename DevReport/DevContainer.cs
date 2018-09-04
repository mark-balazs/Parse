using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevReport
{
    public class DevContainer
    {
        public List<Developer> Container { get; set; }

        public DevContainer()
        {
            Container = new List<Developer>();
        }
        public void AddDeveloper(string name)
        {
            Container.Add(new Developer(name));
        }

        public bool Contains(string match)
        {
            foreach (Developer dev in Container)
            {
                if (match == dev.Name)
                {
                    return true;
                }
            }
            return false;
        }
        public int Index(string name)
        {
            int index = -1;
            foreach(Developer dev in Container)
            {
                index++;
                if (dev.Name == name)
                    return index;
            }
            return -1;
        }
    }
}
