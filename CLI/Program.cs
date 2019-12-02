
using Microsoft.Extensions.CommandLineUtils;
using CLI.Commands;
using System.Reflection;
using System.IO;
using System;

namespace CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Override the arguments to allow debug/breakpoints with arguments
            //args = new string[3];
            //args[0] = "watch";
            //args[1] = "-d";
            //args[2] = @"C:\Users\Lucas\source\repos\zdragon.net\releaseTemp\";

            var app = new CommandLineApplication();
            app.Name = "ckc";
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", "v2.0.6-beta");

            app.OnExecute(() =>
            {
                return 0;
            });

            CommandsBuilder.CreateBuildCommand(app);
            CommandsBuilder.CreateWatchCommand(app);
            CommandsBuilder.CreateServeCommand(app);
            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException commandParsingException)
            {
                app.Error.WriteLine("Error: " + commandParsingException.Message);
                app.ShowHelp();
            }
        }

    }


}
