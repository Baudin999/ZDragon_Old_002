using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Project;
using Project.FileSystems;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;
using System.Net.Http.Headers;

namespace CLI
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "dev_origins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseMiddleware<ErrorLoggingMiddleware>();

            if (ProjectContext.FileSystem?.FileSystemType == FileSystemType.InMemory)
            {
                app.Use(async (context, next) =>
                {
                    var path = (ProjectContext.Instance?.OutPath ?? "") + context.Request.Path;
                    path = path.Replace("//", "/");
                    if (ProjectContext.FileSystem.HasFile(path))
                    {
                        var text = ProjectContext.FileSystem.ReadFileText(path);
                        if (path.EndsWith(".html"))
                        {
                            var result = new FileContentResult(Encoding.ASCII.GetBytes(text), "text/html");
                            await context.WriteResultAsync(result);
                        }
                        else if (path.EndsWith(".js"))
                        {
                            var result = new FileContentResult(Encoding.ASCII.GetBytes(text), "application/javascript");
                            await context.WriteResultAsync(result);
                        }
                        else if (path.EndsWith(".json"))
                        {
                            var result = new FileContentResult(Encoding.ASCII.GetBytes(text), "application/json");
                            await context.WriteResultAsync(result);
                        }
                    }
                    else
                    {
                        await next.Invoke();
                    }
                });
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "wwwroot")),
                RequestPath = ""
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class HttpContextExtensions
    {
        private static readonly RouteData EmptyRouteData = new RouteData();

        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();

        public static Task WriteResultAsync<TResult>(this HttpContext context, TResult result)
            where TResult : IActionResult
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.RequestServices.GetService<IActionResultExecutor<TResult>>();

            if (executor == null)
            {
                throw new InvalidOperationException($"No result executor for '{typeof(TResult).FullName}' has been registered.");
            }

            var routeData = context.GetRouteData() ?? EmptyRouteData;

            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            return executor.ExecuteAsync(actionContext, result);
        }
    }

    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine($"The following error happened: {e.Message}");
                throw;
            }
        }
    }


}
