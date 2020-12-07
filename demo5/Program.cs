using System;
using System.Collections.Concurrent;
using System.Threading;
namespace demo5
{
    class Program
    {
        public static ConcurrentDictionary<string, ProcessPool> TaskProcessMapping = new ConcurrentDictionary<string, ProcessPool>();
 
        static void Main(string[] args)
        {
            Init();
            
            while(true)
            {
                var command = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(command)) break;
                foreach(var tasks in command.Split(",",StringSplitOptions.RemoveEmptyEntries))
                {
                    //1:10 => task_1 : 10 tasks
                    var taskWithCount = tasks.Split(":",StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < int.Parse(taskWithCount[1]); i++)
                    {
                        var taskName = $"Task_{taskWithCount[0]}";
                        var processPool = TaskProcessMapping[taskName];
                        var taskData = $"Id = {i}";
                        Thread.Sleep(TimeSpan.FromMilliseconds(50));
                        processPool.Enqueue(taskData);
                    }
                }
            }
        }

        static void Init()
        {
            TaskProcessMapping = new ConcurrentDictionary<string, ProcessPool>();
            TaskProcessMapping.TryAdd("Task_0",new ProcessPool(new ProcessPoolSetting{ Name = "Task_0", MaxProcess = 2, MinProcess = 0, Color = ConsoleColor.DarkCyan, IdelTimeout = TimeSpan.FromMinutes(1) }));
            TaskProcessMapping.TryAdd("Task_1",new ProcessPool(new ProcessPoolSetting{ Name = "Task_1", MaxProcess = 3, MinProcess = 0, Color = ConsoleColor.DarkBlue, IdelTimeout = TimeSpan.FromSeconds(5)}));
            TaskProcessMapping.TryAdd("Task_2",new ProcessPool(new ProcessPoolSetting{ Name = "Task_2", MaxProcess = 4, MinProcess = 1, Color = ConsoleColor.DarkRed, IdelTimeout = TimeSpan.FromSeconds(20) }));
        }
    }
}
