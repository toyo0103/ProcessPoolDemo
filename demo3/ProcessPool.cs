using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace demo3
{
    public class ProcessPool : IDisposable
    {
        internal BlockingCollection<string> _queue = new BlockingCollection<string>();
        private ConcurrentDictionary<int, Thread> _threads = new ConcurrentDictionary<int, Thread>();
        private TimeSpan _idleTimeout = TimeSpan.FromSeconds(10);
        private CancellationTokenSource _cts;
        private string _name;

        public ProcessPool(string name)
        {
            this._cts = new CancellationTokenSource();
            this._name = name;

            for (int i = 0; i < 3; i++)
            {
                Thread t = new Thread(TaskProcessHandler);
                t.Start();
                _threads.TryAdd(t.ManagedThreadId, t);
            }
        }

        public void Enqueue(string task)
        {
            _queue.Add(task);
        }

        private void TaskProcessHandler()
        {
            var process = this.GenerateProcess();

            while (this._queue.TryTake(out var task, _idleTimeout))
            {
                process.StandardInput.WriteLine($"[{_name} - {Thread.CurrentThread.ManagedThreadId}] Do Task : {task}");
                var response = process.StandardOutput.ReadLine();
                Console.WriteLine(response);
            }

            process.StandardInput.Close();
            process.WaitForExit();
            _threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out var t);
            Console.WriteLine($"[{_name} - {Thread.CurrentThread.ManagedThreadId}] idle timeout");
            Console.WriteLine($"[{_name} - {Thread.CurrentThread.ManagedThreadId}] close process");
        }

        private Process GenerateProcess()
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

        public void Dispose()
        {
            this._queue.CompleteAdding();
            _cts.Cancel();
            foreach (Thread t in _threads.Values) t.Join();
        }
    }
}
