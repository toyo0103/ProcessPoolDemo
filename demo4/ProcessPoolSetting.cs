using System;

namespace demo4
{
    public class ProcessPoolSetting
    {
        public string Name{get;set;}
        public int MaxProcess{get;set;}
        public int MinProcess{get;set;}
        public ConsoleColor Color {get;set;}

        public TimeSpan IdelTimeout {get;set;} = TimeSpan.FromSeconds(5);
    }
}