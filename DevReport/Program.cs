using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainLibrary.Program;
using static System.Console;
using static System.IO.File;
using MainLibrary;


namespace DevReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Reporter reporter = new Reporter(args);
            reporter.MakeReport();
            reporter.Serializer();
        }
    }
}
