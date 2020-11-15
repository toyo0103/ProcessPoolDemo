using System;
using System.Collections.Concurrent;

namespace demo3
{
    class Program
    {
        public static ConcurrentDictionary<string, ProcessPool> TaskProcessMapping = new ConcurrentDictionary<string, ProcessPool>();
 
        static void Main(string[] args)
        {
            Init();
            
            int count = 50;
            for (int i = 0; i < count; i++)
            {
                var taskName = $"Task_{ i%3}";

                var processPool = TaskProcessMapping[taskName];
                var taskData = Guid.NewGuid().ToString();
                processPool.Enqueue(taskData);
            }
        }

        static void Init()
        {
            TaskProcessMapping = new ConcurrentDictionary<string, ProcessPool>();
            TaskProcessMapping.TryAdd("Task_0",new ProcessPool("Task_0"));
            TaskProcessMapping.TryAdd("Task_1",new ProcessPool("Task_1"));
            TaskProcessMapping.TryAdd("Task_2",new ProcessPool("Task_2"));
        }
    }
}
