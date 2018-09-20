using System.Diagnostics;
using System;
using static System.Console;
using static MainLibrary.Program;

namespace Parse
{
    public partial class Program
    {
        private static void Main(string[] args)
        {
            MainClass mainClass = new MainClass(args);
            mainClass.Parse();
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = ("cmd.exe");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.StandardInput.WriteLine("devreport \"" + mainClass.Csv() + '"');
            process.StandardInput.Flush();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            WriteLine(process.StandardOutput.ReadLine());
            WriteLine(process.StandardOutput.ReadLine());
            process.Close();
            process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = ("cmd.exe");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.StandardInput.WriteLine("qareport \"" + mainClass.Csv() + '"');
            process.StandardInput.Flush();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            WriteLine(process.StandardOutput.ReadLine());
            WriteLine(process.StandardOutput.ReadLine());
            process.Close();
        }
    }
}