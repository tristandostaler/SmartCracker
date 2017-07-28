using SmartCracker.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartCracker.ArgumentParsing
{
    public class Options
    {
        public Dictionary<string, Option> SelectedOptions { get; set; }
        private List<Option> _allOptions = new List<Option>();
        private Action _postValidation;

        private Options()
        {
            SelectedOptions = new Dictionary<string, Option>();
            SetupAllOptions();
            try
            {
                ValidateAllOption();
            }
            catch (Exception ex)
            {
                this.PrintUsage();
                Console.WriteLine($"\nAn error occured: {ex.Message}\n");
                throw;
            }
        }

        private void PrintUsage()
        {
            Console.WriteLine(GetUsage());
        }

        private string GetUsage()
        {
            var toReturn = Strings.GreetingHeader;
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

            toReturn += Strings.GreetingFooter;

            return toReturn;
        }
        
        private void SetupAllOptions()
        {
            var generalHelper = new Option(optionName: Strings.Helper, explaination: "Display this help screen", 
                optionType: OptionTypeEnum.Switch, shortArgument: "h", longArgument: "help", validationAction: (option) =>
                {
                    if (option.GivenInput)
                    {
                        this.PrintUsage();
                        Environment.Exit(0);
                    }
                });
            _allOptions.Add(generalHelper);

            var customDictionOption = new Option(optionName: Strings.CustomDictio, explaination: "This is a custom dictionnary that you want to"
                    + " provide that will be added to all other dictionnaries",
                optionType: OptionTypeEnum.String,
                shortArgument: "d", longArgument: "customDict", required: false,
                validationAction: (option) =>
                {
                    if (!File.Exists(option.GivenInput))
                    {
                        throw new Exception("The dictionnary file provided does not exists!");
                    }
                });
            _allOptions.Add(customDictionOption);

            var minCrackingLength = new Option(optionName: Strings.MinimumCrackingLength, explaination: "The minimum password length for the password cracking",
                optionType: OptionTypeEnum.Int,
                longArgument: "minLen", defaultValue: "6", required: false,
                validationAction: (option) =>
                {
                    if (option.GivenInput < 1)
                    {
                        throw new Exception("The minimum cracking length need to be a number and > 0");
                    }
                });
            _allOptions.Add(minCrackingLength);

            var maxCrackingLength = new Option(optionName: Strings.MaximumCrackingLength, explaination: "The maximum password length for the password cracking",
                optionType: OptionTypeEnum.Int,
                longArgument: "maxLen", defaultValue: "12", required: false,
                validationAction: (option) =>
                {
                    if (option.GivenInput < 1)
                    {
                        throw new Exception("The maximum cracking length need to be a number and > 0");
                    }
                });
            _allOptions.Add(maxCrackingLength);

            var cewlUrl = new Option(optionName: Strings.CeWLUrl, explaination: "This is the url to crawl with CeWL",
                optionType: OptionTypeEnum.String,
                shortArgument: "u", longArgument: "url", required: false,
                validationAction: (option) =>
                {
                    if (!File.Exists(option.GivenInput))
                    {
                        throw new Exception("The dictionnary file provided does not exists!");
                    }
                });
            _allOptions.Add(cewlUrl);


            _postValidation = () =>
            {
                if(minCrackingLength.GivenInput > maxCrackingLength.GivenInput)
                {
                    throw new Exception($"{minCrackingLength.Name} needs to be smaller or equal to {maxCrackingLength.Name}!");
                }
                //TODO
            };
        }
        private void ValidateAllOption()
        {
            foreach(var option in _allOptions)
            {
                if (string.IsNullOrEmpty(option.Name) || string.IsNullOrEmpty(option.Explaination))
                {
                    if (!string.IsNullOrEmpty(option.Explaination))
                    {
                        throw new Exception("The name for every option is mandatory! " + option.Explaination);
                    }
                    else
                    {
                        throw new Exception("The name and the explaination for every option is required!");
                    }
                }
                if (string.IsNullOrEmpty(option.ShortArgument) && string.IsNullOrEmpty(option.LongArgument))
                {
                    throw new Exception("You need to provide a short argument, a long argument or both for option " + option.Name);
                }
                if (_allOptions.Where(o =>
                     ((!string.IsNullOrEmpty(o.LongArgument) && !string.IsNullOrEmpty(option.LongArgument) 
                        && o.LongArgument == option.LongArgument) 
                     || (!string.IsNullOrEmpty(o.ShortArgument) && !string.IsNullOrEmpty(option.ShortArgument) 
                        && o.ShortArgument == option.ShortArgument))
                     && o != option).ToList().Count != 0)
                {
                    throw new Exception("All arguments, short or long, needs to be unique for option " + option.Name);
                }
            }
        }
        private void ValidateSelectedOptions()
        {
            foreach(var requiredOption in _allOptions.Where(o => o.Required))
            {
                if (!SelectedOptions.Values.Contains(requiredOption))
                {
                    throw new Exception($"You are missing required options! The following option is required: {requiredOption.Name}");
                }
            }

            foreach (var option in SelectedOptions.Values)
            {
                option.ValidateAction();
            }
            
            _postValidation();
        }

        private void ParseOptions(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                foreach(var option in _allOptions)
                {
                    if($"-{option.ShortArgument}".ToLower() == arg.ToLower() || $"--{option.LongArgument}".ToLower() == arg.ToLower())
                    {
                        SelectedOptions.Add(option.Name, option);
                        switch(option.Type)
                        {
                            case OptionTypeEnum.Double:
                                if(i+1 >= args.Length)
                                {
                                    throw new Exception($"The option {option.Name} needs an input following the argument.");
                                }
                                option.GivenInput = double.Parse(args[i + 1]);
                                i++;
                                break;
                            case OptionTypeEnum.Int:
                                if (i + 1 >= args.Length)
                                {
                                    throw new Exception($"The option {option.Name} needs an input following the argument.");
                                }
                                option.GivenInput = int.Parse(args[i + 1]);
                                i++;
                                break;
                            case OptionTypeEnum.String:
                                if (i + 1 >= args.Length)
                                {
                                    throw new Exception($"The option {option.Name} needs an input following the argument.");
                                }
                                option.GivenInput = args[i+1];
                                i++;
                                break;
                            case OptionTypeEnum.Switch:
                                option.GivenInput = true;
                                break;
                        }
                        break;
                    }
                }
            }
        }


        public static Options Parse(string[] args)
        {
            var options = new Options();
            if (args.Length == 0)
            {
                options.PrintUsage();
                Environment.Exit(0);
            }
            try
            {
                options.ParseOptions(args);
                options.ValidateSelectedOptions();
            }
            catch (Exception ex)
            {
                options.PrintUsage();
                Console.WriteLine($"\nAn error occured: {ex.Message}");
                Console.WriteLine($"\nThe StackTrace was: \n{ex.StackTrace}\n");
                Environment.Exit(0);
            }
            return options;
        }
    }
}
