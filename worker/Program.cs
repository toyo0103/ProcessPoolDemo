using System;

namespace worker
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                string line = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(line)) break;

                Console.WriteLine(line);
            }while(true);
        }
    }
}
