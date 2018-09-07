using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary
{
    public class QAContainer
    {
        public List<Tester> Container { get; set; }

        public QAContainer()
        {
            Container = new List<Tester>();
        }

        public bool Contains(string match)
        {
            foreach (Tester dev in Container)
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
            foreach (Tester dev in Container)
            {
                index++;
                if (dev.Name == name)
                    return index;
            }
            return -1;
        }
    }
}
