using System;
using System.Diagnostics;

namespace SmartCracker.OSCommands
{
    public class UbuntuOSCommandHandler : IOSCommandHandler
    {
        private bool keepRunning = true;
        private Process CreateProcess(string command, bool interactive = false){
            keepRunning = true;
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                keepRunning = false;
            };

            var process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command.Replace("\"","\\\"")}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            if(interactive)
            {
                process.StartInfo.RedirectStandardInput = true;
            }
            
            Console.WriteLine($"\nRunning {command}:");
            return process;
        }

        public string Run(string command, bool interactive = false, bool displayLive = false)
        {
            try
            {
                var process = CreateProcess(command, interactive);
                process.Start();

                var totalOutput = "";
                while(!process.HasExited && keepRunning){
                    string outPut = "\t" + process.StandardOutput.ReadLine();
                    totalOutput += outPut;
                    if(displayLive){
                        Console.WriteLine(outPut);
                    }
                }
                if(!keepRunning && !process.HasExited)
                {
                    process.Kill();
                }

                process.WaitForExit();
                var exitCode = process.ExitCode;
                if(displayLive){
                    Console.WriteLine($"Exit code: {exitCode}");
                }
                
                process.Dispose();

                return totalOutput;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured while running the command: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return "Exited the process with an exception";
            }
        }
    }
}