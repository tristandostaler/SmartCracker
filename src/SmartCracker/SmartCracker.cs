using System;
using SmartCracker.OSCommands;
using SmartCracker.ArgumentParsing;

namespace SmartCracker
{
    public class SmartCracker
    {
        public static void Main(string[] args)
        {
            var options = Options.Parse(args);
        }
    }
}
