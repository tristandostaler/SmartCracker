﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartCracker.ArgumentParsing
{
    public class Options
    {
        private string _greetingHeader = @"
 .oooooo..o                                          .     .oooooo.                                oooo                           
d8P'    `Y8                                        .o8    d8P'  `Y8b                               `888                           
Y88bo.      ooo. .oo.  .oo.    .oooo.   oooo d8b .o888oo 888          oooo d8b  .oooo.    .ooooo.   888  oooo   .ooooo.  oooo d8b 
 `'Y8888o.  `888P'Y88bP'Y88b  `P  )88b  `888''8P   888   888          `888''8P `P  )88b  d88' `'Y8  888 .8P'   d88' `88b `888''8P 
     `'Y88b  888   888   888   .oP'888   888       888   888           888      .oP'888  888        888888.    888ooo888  888     
oo     .d8P  888   888   888  d8(  888   888       888 . `88b    ooo   888     d8(  888  888   .o8  888 `88b.  888    .o  888     
8''88888P'  o888o o888o o888o `Y888''8o d888b      '888'  `Y8bood8P'  d888b    `Y888''8o `Y8bod8P' o888o o888o `Y8bod8P' d888b    
                                                                                                                                                                                                                                    
                                                  Designed by Tristan Dostaler                                                   

";
        private string _greetingFooter = @"
Example of usage:
    SmartCracker -u http://www.example.com -min 6 -max 10 hash.txt
";

        public static Options Parse(string[] args)
        {
            var options = new Options();
            options.ParseOptions(args);
            try
            {
                options.ValidateAllOptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine(options.GetUsage());
                Console.WriteLine($"\nAn error occured: {ex.Message}\n");
                throw;
            }
            return options;
        }

        public List<Option> SelectedOptions { get; set; }

        private List<Option> _allOptions = new List<Option>();

        private string GetUsage()
        {
            var toReturn = _greetingHeader;
            foreach (var option in _allOptions)
            {
                toReturn += $"\t";
                if(!string.IsNullOrEmpty(option.ShortArgument))
                {
                    toReturn += $"-{option.ShortArgument}";
                    if(!string.IsNullOrEmpty(option.LongArgument))
                    {
                        toReturn += " | ";
                    }
                }
                toReturn += (string.IsNullOrEmpty(option.LongArgument) ? "" : $"--{option.LongArgument}")
                    + $"\n\t\tRequired: {option.Required.ToString()}\n\t\t{option.Explaination}"
                    + (string.IsNullOrEmpty(option.DefaultValue) ? "" : $"\n\t\tDefault value: {option.DefaultValue}")
                    + "\n";
            }

            toReturn += _greetingFooter;

            return toReturn;
        }

        public Options()
        {
            SelectedOptions = new List<Option>();
            SetupAllOptions();
        }

        private void SetupAllOptions()
        {
            _allOptions.Add(new Option(explaination: "This is a custom dictionnary that you want to" 
                    + " provide that will be added to all other dictionnaries", 
                optionType: OptionTypeEnum.String,
                shortArgument: "d", longArgument: "customDict", required: false, 
                validationAction: (option) =>
                {
                    if (!File.Exists(option.GivenInput))
                    {
                        throw new Exception("The dictionnary file provided does not exists!");
                    }
                }));
            _allOptions.Add(new Option(explaination: "The minimum password length for the password cracking", 
                optionType: OptionTypeEnum.Int,
                longArgument: "minLen", defaultValue: "6", required: false, 
                validationAction: (option) =>
                {
                    int outValue = -1;
                    if(!int.TryParse(option.GivenInput, out outValue) || outValue < 1)
                    {
                        throw new Exception("The minimum cracking length need to be a number and > 0");
                    }
                }));
            _allOptions.Add(new Option(explaination: "The maximum password length for the password cracking",
                optionType: OptionTypeEnum.Int,
                longArgument: "maxLen", defaultValue: "12", required: false, 
                validationAction: (option) =>
                {
                    int outValue = -1;
                    if (!int.TryParse(option.GivenInput, out outValue) || outValue < 1)
                    {
                        throw new Exception("The maximum cracking length need to be a number and > 0");
                    }
                }));
            _allOptions.Add(new Option(explaination: "This is the url to crawl with CeWL", 
                optionType: OptionTypeEnum.String,
                shortArgument: "u", longArgument: "url", required: false, 
                validationAction: (option) =>
                {
                    if (!File.Exists(option.GivenInput))
                    {
                        throw new Exception("The dictionnary file provided does not exists!");
                    }
                }));
            
        }

        private void ValidateAllOptions()
        {
            foreach(var requiredOption in _allOptions.Where(o => o.Required).ToList())
            {
                if (!SelectedOptions.Contains(requiredOption))
                {
                    throw new Exception($"You are missing required options! The following option is required: {requiredOption.Explaination}");
                }
            }

            foreach (var option in SelectedOptions)
            {
                if(string.IsNullOrEmpty(option.ShortArgument) && string.IsNullOrEmpty(option.LongArgument))
                {
                    throw new Exception("You need to provide a short argument, a long argument or both for option " + option.Explaination);
                }
                if(_allOptions.Where(o => 
                    (o.LongArgument == option.LongArgument || o.ShortArgument == option.ShortArgument) 
                    && o != option).ToList().Count != 0)
                {
                    throw new Exception("All arguments, short or long, needs to be unique for option " + option.Explaination);
                }
                if(option.Required)
                {
                    option.ValidateAction();
                } 
            }
        }

        private void ParseOptions(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                foreach(var option in _allOptions)
                {
                    if(option.ShortArgument == arg || option.LongArgument == arg)
                    {
                        SelectedOptions.Add(option);
                        switch(option.Type)
                        {
                            case OptionTypeEnum.Double:
                            case OptionTypeEnum.Int:
                            case OptionTypeEnum.String:
                                option.GivenInput = args[i+1];
                                break;
                            case OptionTypeEnum.Switch:
                                break;
                        }
                        continue;
                    }
                }
            }
        }
    }
}
