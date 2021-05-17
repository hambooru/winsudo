using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace winsudo
{
    class Program
    {
        [DllImport("shell32.dll")]
        internal static extern bool IsUserAnAdmin();

        static void Main(string[] args)
        {
            String userName = Environment.UserName;
            string path = Directory.GetCurrentDirectory();

            if (args.Length == 0)
            {
                System.Console.WriteLine("winsudo: no argument provided");
            }
            else
            {
                if (!Elevate(System.Reflection.Assembly.GetExecutingAssembly().Location))
                {
                    System.Console.WriteLine("winsudo: elevation prompt refused.");
                }
                System.Console.Write("[winsudo] password for " + userName + ": ");
                string password = null;
                while (true)
                {
                    var key = System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }
                if (password == "workinprogress") 
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("winsudo: requesting elevation...");
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = @"/c " + args[0];
                    process.StartInfo = startInfo;
                    process.Start();
                }
                else
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("winsudo: incorrect password.");
                }
                
            }
        }
        
        private static bool Elevate(string processPath)
        {
            bool elevated = false;
            if (!IsUserAnAdmin())
            {
                elevated = true;
                ProcessStartInfo procInfo = new ProcessStartInfo(processPath);
                procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                procInfo.UseShellExecute = true;
                procInfo.Verb = "runas";
                Process.Start(procInfo);
            }
            return elevated;
        }
    }
}