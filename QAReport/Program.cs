using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainLibrary;

namespace QAReport
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] a = { "a" };
            QAReporter reporter = new QAReporter(a);
            reporter.MakeReport();
        }
    }
}
