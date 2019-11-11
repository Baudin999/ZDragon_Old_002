using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CLI
{
    public class WebServer
    {
        public static Task Start(string rootPath)
        {

            return Task.Run(() =>
            {
                var webHost = WebHost
                .CreateDefaultBuilder(new string[] { })
                .Configure(config =>
                {
                    config.UseStaticFiles();
                    config.UseWelcomePage("/index.html");
                    var server = config.ApplicationServices.GetRequiredService<IServer>();
                    var addresses = server.Features?.Get<IServerAddressesFeature>()?.Addresses;
                    Console.WriteLine(string.Join(", ", addresses));
                })
                .UseWebRoot(rootPath)
                .Build();

                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    WebServer.OpenBrowser("http://localhost:5000/index.html");
                });
                webHost.Run();
            });
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}")); // Works ok on windows and escape need for cmd.exe
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url); // Not tested
            }
            else
            {
                // Nothing
            }

        }
    }
}