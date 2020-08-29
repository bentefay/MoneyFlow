using System;
using System.Linq;
using LanguageExt;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
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
        const string SeqUrl = "http://localhost:5002";
        
        public static int Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(SeqUrl)
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
                            
                            if (env.IsDevelopment())
                                LogServiceUrls(logger, app);
                            
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

        private static void LogServiceUrls(ILogger logger, IApplicationBuilder app)
        {
            logger.Information(
               "\n=================\n" +
               "Access dependencies at the following locations:\n" +
               "- MoneyFlow @ {MoneyFlowUrl}\n" +
               "- Seq       @ {SeqUrl}\n" +
               "=================", 
                app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault(x => x.StartsWith("http://")), SeqUrl);
        }
    }
}
