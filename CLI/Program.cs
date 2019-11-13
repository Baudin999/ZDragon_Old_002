
using Microsoft.Extensions.CommandLineUtils;
using CLI.Commands;
using System.IO;

namespace CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ZDragon.NET";
            app.HelpOption("-?|-h|--help");

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
