
using Microsoft.Extensions.CommandLineUtils;
using CLI.Commands;

namespace CLI
{
    public class Program
    {
        public Program()
        {
        }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ZDragon.NET";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() => {
                //Console.WriteLine("Hello World!");
                return 0;
            });



            CommandsBuilder.CreateBuildCommand(app);
            CommandsBuilder.CreateWatchCommand(app);


            app.Execute(args);
        }
    }
}
