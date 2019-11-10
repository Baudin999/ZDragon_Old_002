using System;
using Microsoft.Extensions.CommandLineUtils;
using CLI;

namespace CLI.Commands
{
    public partial class CommandsBuilder
    {

        public static void CreateBuildCommand(CommandLineApplication app)
        {
            _ = app.Command("build", (command) =>
                  {
                      command.Description = "Build a .car file and output the result.";
                      command.HelpOption("-?|-h|--help");

                      var fileOption = command.Option(
                          "-f|--file <filePath>",
                          "The path to the file which should be built.",
                          CommandOptionType.SingleValue);

                      var xsdFlag = command.Option(
                          "--xsd",
                          "Generate XSD",
                          CommandOptionType.NoValue);

                      command.OnExecute(() =>
                          {
                              var filePath = fileOption.Value();
                              if (filePath is null)
                              {
                                  Console.WriteLine(@"
When using the build command you should always specify a file or a project.
Neither was supplied. Canceling command.

Example:
ckc build -f ./something.car

");
                              }

                              try
                              {
                                  //var text = System.IO.File.ReadAllText(filePath);
                                  //Transpiler transpiler = new Transpiler(text);
                                  //Console.WriteLine(transpiler.XsdToString());
                              }
                              catch (Exception ex)
                              {
                                  Console.WriteLine(ex.Message);
                              }


                              return 0;
                          });
                  });
        }
    }
}
