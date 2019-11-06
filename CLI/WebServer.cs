using System;
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
                    config.UseWelcomePage();
                    var server = config.ApplicationServices.GetRequiredService<IServer>();
                    var addresses = server.Features?.Get<IServerAddressesFeature>()?.Addresses;
                    Console.WriteLine(string.Join(", ", addresses));
                })
                .UseWebRoot(rootPath)
                .Build();

                webHost.Run();
            });
        }
    }
}
