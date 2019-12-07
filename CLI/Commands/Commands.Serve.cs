using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace CLI.Commands
{
    public partial class CommandsBuilder
    {

        public static void CreateServeCommand(CommandLineApplication app)
        {
            _ = app.Command("serve", (command) =>
            {
                command.Description = "Serve the content of a Project";
                command.HelpOption("-?|-h|--help");

                var fileOption = command.Option(
                      "-f|--file <filePath>",
                      "The path to the file which should be built.",
                      CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    Console.WriteLine(@"
Welcome to ZDragon!

To quit the application press 'q'
");
                    var directory = fileOption.HasValue() switch
                    {
                        false => Directory.GetCurrentDirectory(),
                        true => fileOption.Value()
                    };

                    var project = new Project(directory);
                    WebServer.Start(project.OutPath);

                    // Wait for the user to quit the program.
                    Console.WriteLine("Press 'q' to quit the sample.");
                    while (Console.Read() != 'q') { }
                    project.Dispose();
                    return 0;
                });
            });
        }


    }
}
