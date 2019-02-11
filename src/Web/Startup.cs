using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Web.Utils;
using Web.Utils.Serialization.Serializers;
using Web.Utils.Serialization.TypeConverters;

namespace Web
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            TinyTypeTypeConverter.ScanForAndRegisterTinyTypeTypeConverters(typeof(Startup));
            
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(setup => setup
                    .SerializerSettings
                    .ConfigureForApiControllers(_logger));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app
                .UseMiddleware<SerilogMiddleware>()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseHttpsRedirection()
                .UseMvc();
        }
    }
}
