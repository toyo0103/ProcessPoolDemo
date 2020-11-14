using System;
using System.Diagnostics;

namespace demo1
{
    class Program
    {
        static void Main(string[] args)
        {
            //StartProcess();
            LoopStartProcess(50);

        }

        private static void LoopStartProcess(int count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                StartProcess();
            }
            sw.Stop();
            Console.WriteLine($"start process {count} times, ElapsedMilliseconds : {sw.ElapsedMilliseconds}");
        }

        static void StartProcess()
        {
            var fileName = "dotnet";
            var arguments = "./worker/bin/Debug/netcoreapp3.1/worker.dll ";
            Process p = Process.Start(new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            });

            p.PriorityClass = ProcessPriorityClass.Normal;
            p.Start();
            p.StandardInput.WriteLine($"hello worker");
            
            var response = p.StandardOutput.ReadLine();
            Console.WriteLine(response);

            p.StandardInput.Close();
            p.WaitForExit();          
        }
    }
}
