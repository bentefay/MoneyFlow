using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Web.Functions;
using Web.Utils;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;

namespace Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5002")
                .CreateLogger();

            Log.Logger = logger;

            try
            {
                CreateHostBuilder(args, logger)
                    .Build()
                    .Run();

                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, ILogger logger) =>
            Host
                .CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices((context, services) =>
                        {
                            var config = context.Configuration;
                            services
                                .AddSingleton(logger)
                                .AddSingleton(config.GetStorageConnectionString())
                                .AddControllers(builder =>
                                    builder.Filters.Add<SerilogMvcLoggingAttribute>())
                                .AddNewtonsoftJson(setup => setup
                                    .SerializerSettings
                                    .ConfigureForApiControllers(logger));
                        })
                        .Configure((context, app) =>
                        {
                            var env = context.HostingEnvironment;
                            app
                                .InDevelopment(env, x => x
                                    .UseDeveloperExceptionPage())
                                .NotInDevelopment(env, x => x
                                    .UseHsts())
                                .NotInDevelopment(env, x => x
                                    .UseDefaultFiles())
                                .UseStaticFiles()
                                .NotInDevelopment(env, x => x
                                    .UseHttpsRedirection())
                                .UseSerilogRequestLogging()
                                .UseRouting()
                                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                                .InDevelopment(env, x => x
                                    .UseProxyToSpaDevelopmentServer("http://localhost:1234"));
                        });
                });
    }
}
