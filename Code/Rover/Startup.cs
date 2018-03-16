using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using WALLE.Link.Extensions;
using WALLE.Rover.Dto;
using WALLE.Rover.Dto.Telemetry;
using WALLE.Rover.Units;

namespace WALLE.Rover
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.private.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddSingleton(Configuration);
                services.AddHttpLink();
                services.AddLinkLogger();

                services.AddMvc();
                services.AddSwaggerGen(config =>
                {
                    config.SwaggerDoc("v1", new Info { Title = "WALL-E Rover API", Version = "v1" });
                    config.DescribeAllEnumsAsStrings();
                });

                services.AddSingleton<TelemetryData>();
                services.AddSingleton<IHostedService, TelemetryProcessor>();
                services.AddSingleton<IHostedService, CommandsProcessor>();
                services.AddSingleton<UnitsController, UnitsController>();

                _logger.LogInformation($"{nameof(ConfigureServices)} is done");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                app.UseLinkLogger();

                app.UseMvc();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WALL-E Rover API");
                });

                _logger.LogInformation($"{nameof(Configure)} is done");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
