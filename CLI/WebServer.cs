using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CLI
{
    public class WebServer
    {
        public static string RootPath;
        public static Task Start(string rootPath)
        {
            RootPath = rootPath;
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                WebServer.OpenBrowser("https://localhost:5001/index.html");
            });
            return Task.Run(() =>
            {
                Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                            .UseStartup<Startup>()
                            .UseWebRoot(rootPath)
                            .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
                            .ConfigureLogging(logging => logging.ClearProviders());
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .Build()
                    .Run();

            });
        }

        public static void OpenBrowser(string url)
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
    }
}