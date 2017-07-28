using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCracker.Resources
{
    public class Strings
    {
        public static string GreetingHeader { get; set; } = @"
 .oooooo..o                                          .     .oooooo.                                oooo                           
d8P'    `Y8                                        .o8    d8P'  `Y8b                               `888                           
Y88bo.      ooo. .oo.  .oo.    .oooo.   oooo d8b .o888oo 888          oooo d8b  .oooo.    .ooooo.   888  oooo   .ooooo.  oooo d8b 
 `'Y8888o.  `888P'Y88bP'Y88b  `P  )88b  `888''8P   888   888          `888''8P `P  )88b  d88' `'Y8  888 .8P'   d88' `88b `888''8P 
     `'Y88b  888   888   888   .oP'888   888       888   888           888      .oP'888  888        888888.    888ooo888  888     
oo     .d8P  888   888   888  d8(  888   888       888 . `88b    ooo   888     d8(  888  888   .o8  888 `88b.  888    .o  888     
8''88888P'  o888o o888o o888o `Y888''8o d888b      '888'  `Y8bood8P'  d888b    `Y888''8o `Y8bod8P' o888o o888o `Y8bod8P' d888b    
                                                                                                                                                                                                                                    
                                                  Designed by Tristan Dostaler                                                   

";
        public static string GreetingFooter { get; set; } = @"
Example of usage:
    SmartCracker -u http://www.example.com -min 6 -max 10 hash.txt
";

        public static string MinimumCrackingLength { get; set; } = "Minimum cracking length";
        public static string MaximumCrackingLength { get; set; } = "Maximum cracking length";
        public static string CustomDictio { get; set; } = "Custom dictio";
        public static string CeWLUrl { get; set; } = "CeWL URl";
        public static string Helper { get; set; } = "Helper";
    }
}
