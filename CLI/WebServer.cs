using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CLI.Controllers;
using System.Reflection;
using Microsoft.Extensions.Hosting;


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
                WebServer.OpenBrowser("http://localhost:5000/index.html");
            });
            return Task.Run(() =>
            {
                Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                            .UseStartup<Startup>()
                            .UseWebRoot(rootPath);
                    })
                    .Build()
                    .Run();

                //var webHost = WebHost.CreateDefaultBuilder();
                //var webHost = WebHost
                //    .CreateDefaultBuilder(new string[] { })
                //    //.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
                //    .ConfigureServices(services =>
                //    {
                //        services
                //            .AddMvcCore()
                //            .AddApplicationPart(Assembly.GetEntryAssembly());

                //    })
                //    .Configure(config =>
                //    {
                //        config.UseStaticFiles();
                //        config.UseWelcomePage("/index.html");

                //        //var server = config.ApplicationServices.GetRequiredService<IServer>();
                //        //var addresses = server.Features?.Get<IServerAddressesFeature>()?.Addresses;

                //        //Console.WriteLine("\nWebServer:");
                //        //Console.WriteLine($"Dev server running on: http://localhost:5000");
                //        //Console.WriteLine($"Dev server running on: https://localhost:5001");

                //    })
                //    .ConfigureAppConfiguration(app =>
                //    {
                //        //
                //    })
                //    .UseWebRoot(rootPath)
                //    //.ConfigureLogging(logging => logging.ClearProviders())
                //    .Build();


                //webHost.Run();
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