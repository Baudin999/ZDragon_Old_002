using System;
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

                    if (fileOption.Value() is null)
                    {
                        throw new Exception("Should specify a project root directory.");
                    }

                    var project = new Project(fileOption.Value());
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
