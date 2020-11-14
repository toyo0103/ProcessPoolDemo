using System;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace demo1
{
    class Program
    {
        public static ConcurrentDictionary<string, Process> TaskProcessMapping = new ConcurrentDictionary<string, Process>();
 
        static void Main(string[] args)
        {
            Init();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            int count = 50;
            for (int i = 0; i < count; i++)
            {
                if(TaskProcessMapping.ContainsKey("Task_1") == true)
                {
                    var process = TaskProcessMapping["Task_1"];

                    process.StandardInput.WriteLine($"hello {i}");
                    var response = process.StandardOutput.ReadLine();
                    Console.WriteLine(response);
                }
            }

            sw.Stop();
            Console.WriteLine($"start process {count} times, ElapsedMilliseconds : {sw.ElapsedMilliseconds}");
            Clear();
        }

        static void Init()
        {
            TaskProcessMapping = new ConcurrentDictionary<string, Process>();
            TaskProcessMapping.TryAdd("Task_1",GenerateProcess());
        }

        static Process GenerateProcess()
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

            return p;       
        }

        static void Clear()
        {
            foreach(var mapping in TaskProcessMapping) 
            {
                mapping.Value.StandardInput.Close();
                mapping.Value.WaitForExit();
            }
        }
    }
}
