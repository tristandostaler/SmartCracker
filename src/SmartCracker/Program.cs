using SmartCracker.ArgumentParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = Options.Parse(args);
        }
    }
}
