using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace CLI.Commands
{
    public partial class CommandsBuilder
    {

        public static void CreateWatchCommand(CommandLineApplication app)
        {
            _ = app.Command("watch", (command) =>
                  {
                      command.Description = "Watch a .car Project";
                      command.HelpOption("-?|-h|--help");

                      var fileOption = command.Option(
                            "-f|--file <filePath>",
                            "The path to the file which should be built.",
                            CommandOptionType.SingleValue);

                      var serve = command.Option(
                            "-s|--serve",
                            "Serve files from the out folder in a simple static file server.",
                            CommandOptionType.NoValue);

                      command.OnExecute(() =>
                      {

                          if (fileOption.Value() is null)
                          {
                              throw new Exception("Should specify a project root directory.");
                          }

                          var project = new Project(fileOption.Value());

                          Task webserverTask = null;
                          if (serve.HasValue())
                          {
                              webserverTask = WebServer.Start(project.OutPath);
                          }

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
