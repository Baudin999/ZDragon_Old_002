using System;
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

                    command.OnExecute(() =>
                    {
                        return 0;
                    });
                });
        }
    }
}
