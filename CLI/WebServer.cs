using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project;

namespace CLI
{
    public static class WebServer
    {
        public static string RootPath = "";
        public static Task Start(string rootPath)
        {
            var portNumber = ProjectContext.Instance?.CarConfig?.PortNumber ?? "5000";
            RootPath = rootPath;
            Task.Run(async () => 
            {
                await Task.Delay(1500);
                WebServer.OpenBrowser($"http://localhost:{portNumber}/index.html");
            });

            var project = ProjectContext.Instance;
            if (project != null)
            {
                WebServer.CreateAssets(project.OutPath);
            }

            return Task.Run(() =>
            {
                System.Console.WriteLine($@"
A local server has been started on:
http://localhost:{portNumber}/
");
                Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                            .UseStartup<Startup>()
                            .UseWebRoot(rootPath)
                            .UseUrls($"http://localhost:{portNumber}/")
                            .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
                            .ConfigureLogging(logging => logging.ClearProviders());

                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .Build()
                    .Run();
            });
        }

        private static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url); 
            }
            else
            {
                // Nothing
            }

        }

        private static void CreateAssets(string outPath)
        {
            AssetHelpers.ReadAndWriteAsset("mermaid.min.js", outPath);
            AssetHelpers.ReadAndWriteAsset("mermaid.min.js.map", outPath);
        }

        
    }

   
}