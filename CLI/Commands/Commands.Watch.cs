﻿using System;
using System.IO;
using System.Threading.Tasks;
using CLI.Signals;
using Microsoft.Extensions.CommandLineUtils;
using Project;

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
                            "-d|--dir <directory>",
                            "The directory of your zdragon project.",
                            CommandOptionType.SingleValue);

                      var serve = command.Option(
                            "-s|--serve",
                            "Serve files from the out folder in a simple static file server.",
                            CommandOptionType.NoValue);

                      command.OnExecute(() =>
                      {
                          Console.WriteLine("\nTo quit the application press 'q'");
                          var directory = fileOption.HasValue() switch
                          {
                              false => Directory.GetCurrentDirectory(),
                              true => fileOption.Value()
                          };
                          //ProjectContext.Init(directory);
                          ProjectContext.InitInMemory();
                          var project = ProjectContext.Instance;
                          if (project != null)
                          {
                              SignalSingleton.ExitSignal.Subscribe(() => {
                                  project.Dispose();
                              });

                              Task? webserverTask = null;
                              if (serve.HasValue())
                              {
                                  webserverTask = WebServer.Start(project.OutPath);
                              }

                              project.Watch();

                              while (Console.ReadKey().Key != ConsoleKey.Q) { }
                              Console.WriteLine();

                              SignalSingleton.ExitSignal.Dispatch();
                              return 0;
                          }
                          else
                          {
                              Console.WriteLine("Could not initialize the project.");
                              return 1;
                          }
                      });
                  });
        }
    }
}
