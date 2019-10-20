using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Web.Utils;
using Web.Utils.Serialization.Serializers;

namespace Web
{
    public class Startup
    {
        private readonly ILogger _logger = Log.Logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson(setup => setup
                    .SerializerSettings
                    .ConfigureForApiControllers(_logger));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
