using System;
using System.Collections.Concurrent;
using System.Threading;
namespace demo4
{
    class Program
    {
        public static ConcurrentDictionary<string, ProcessPool> TaskProcessMapping = new ConcurrentDictionary<string, ProcessPool>();
 
        static void Main(string[] args)
        {
            Init();
            
            int count = 50;
            Random rnd =new Random();

            for (int i = 0; i < count; i++)
            {
                var taskName = $"Task_{ i%3}";
                var processPool = TaskProcessMapping[taskName];
                var taskData = Guid.NewGuid().ToString();
                processPool.Enqueue(taskData);
                //模擬真實 Task 進來的隨機時間
                Thread.Sleep(rnd.Next(50,100));
            }
        }

        static void Init()
        {
            TaskProcessMapping.TryAdd("Task_0",new ProcessPool(new ProcessPoolSetting{ Name = "Task_0", MaxProcess = 2, MinProcess = 0, Color = ConsoleColor.DarkCyan }));
            TaskProcessMapping.TryAdd("Task_1",new ProcessPool(new ProcessPoolSetting{ Name = "Task_1", MaxProcess = 3, MinProcess = 0, Color = ConsoleColor.DarkBlue}));
            TaskProcessMapping.TryAdd("Task_2",new ProcessPool(new ProcessPoolSetting{ Name = "Task_2", MaxProcess = 4, MinProcess = 1, Color = ConsoleColor.DarkRed }));
        }
    }
}
