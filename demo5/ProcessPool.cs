using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace demo5
{
    public class ProcessPool : IDisposable
    {
        internal BlockingCollection<string> _queue = new BlockingCollection<string>();
        private ConcurrentDictionary<int, Thread> _threads = new ConcurrentDictionary<int, Thread>();
        private TimeSpan _idleTimeout;
        private CancellationTokenSource _cts;
        private string _name;

        private ConsoleColor _consoleColor;

        private int _totalProcessCount = 0;
        private int _runningProcessCount = 0;
        private int _maxProcess = 0;
        private int _minProcess = 0;
        private object _root = new object();
        private Thread _monitorThread;
        public ProcessPool(ProcessPoolSetting setting)
        {
            this._cts = new CancellationTokenSource();
            this._name = setting.Name;
            this._maxProcess = setting.MaxProcess;
            this._minProcess = setting.MinProcess;
            this._consoleColor = setting.Color;
            this._idleTimeout = setting.IdelTimeout;

            _monitorThread = new Thread(MonitorBody);
            _monitorThread.Start();

        }

        public void Enqueue(string task)
        {
            _queue.Add(task);
            this.TryIncreaseProcess();
        }

        private bool TryIncreaseProcess()
        {
            lock (_root)
            {
                if (_runningProcessCount < _totalProcessCount) return false;
                if (_totalProcessCount >= _maxProcess) return false;
            }

            Thread t = new Thread(TaskProcessHandler);
            t.Start();
            _threads.TryAdd(t.ManagedThreadId, t);

            return true;
        }

        private void TaskProcessHandler()
        {
            lock (_root) _totalProcessCount++;

            var process = this.GenerateProcess();
            TryTaskTask:
            while (this._queue.TryTake(out var task, _idleTimeout))
            {
                lock (_root) _runningProcessCount++;
                
                process.StandardInput.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{_name} - {Thread.CurrentThread.ManagedThreadId}] Do Task : {task}");
                
                //模擬 Task 執行時間
                Random rnd = new Random();
                Thread.Sleep(TimeSpan.FromMilliseconds(rnd.Next(200,500)));
                
                var response = process.StandardOutput.ReadLine();
                lock (_root) _runningProcessCount --;
            }
            
            if (_totalProcessCount <= _minProcess)
            {
                goto TryTaskTask;
            } 

            lock (_root) _totalProcessCount--;

            process.StandardInput.Close();
            process.WaitForExit();
            _threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out var t);
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
            //p.ProcessorAffinity = (IntPtr)0x0001;
            p.PriorityClass = ProcessPriorityClass.Normal;
            p.Start();   

            return p;       
        }

        public void MonitorBody()
        {
            while(_cts.IsCancellationRequested == false)
            {
                var client = new RestClient("http://localhost:5000/report/metrics");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");

                var data = new 
                {
                    TaskName = _name,
                    ProcessCount = _threads.Count,
                    QueueLength = _queue.Count
                };
                request.AddParameter("application/json", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public void Dispose()
        {
            this._queue.CompleteAdding();
            _cts.Cancel();
            _monitorThread.Join();
            foreach (Thread t in _threads.Values) t.Join();
        }

        public static object WriteTextObj = new object();
    }
}
