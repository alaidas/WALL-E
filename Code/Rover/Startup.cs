using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WALLE.Link;

namespace WALLE.Rover
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILinkClient, LinkClientHttp>();
            services.AddMvc();

            Task.Run(async () =>
            {
                try
                {
                    using (ServiceProvider sp = services.BuildServiceProvider())
                    {
                        var linkClient = sp.GetService<ILinkClient>();

                        //using (linkClient.SubscribeForEvents("none", e => _logger.LogInformation($"Received event id: '{e.Id}', created at: '{e.CreationTime}'")))
                        {
                            while (true)
                            {
                                linkClient.SendEventAsync(new Link.Dto.Event
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    CreationTime = DateTime.UtcNow,
                                    Sender = nameof(Rover)
                                });
                                await Task.Delay(1000);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
