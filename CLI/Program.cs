
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

            var app = new CommandLineApplication();
            app.Name = "ckc";
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", "0.0.1"); //TODO dynamic solution

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

        public static void ReadStuff()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CLI.Assets.style.css";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

        }
    }


}
