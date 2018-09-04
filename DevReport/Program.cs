using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Parse.Program;
using static System.Console;
using static System.IO.File;


namespace DevReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Reporter reporter = new Reporter(args);
            reporter.MakeReport();
        }
    }
}
