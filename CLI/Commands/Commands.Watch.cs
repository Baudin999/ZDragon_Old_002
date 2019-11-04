using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace CLI.Commands
{
    public partial class CommandsBuilder
    {

        public static void CreateWatchCommand(CommandLineApplication app)
        {
            app.Command("watch", (command) =>
                {
                    command.Description = "Watch a .car Project";
                    command.HelpOption("-?|-h|--help");

                    var fileOption = command.Option(
                          "-f|--file <filePath>",
                          "The path to the file which should be built.",
                          CommandOptionType.SingleValue);

                    command.OnExecute(() =>
                    {
                        var project = new Project(fileOption.Value());
                        project.Watch();
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
