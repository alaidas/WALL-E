using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WALLE.Link.Logging;

namespace WALLE.Link.Extensions
{
    public static class LinkApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLinkLogger(this IApplicationBuilder app)
        {
            var logProvider = app.ApplicationServices.GetService<LinkLoggerProvider>();
            var factory = app.ApplicationServices.GetService<ILoggerFactory>();

            factory.AddProvider(logProvider);
            return app;
        }
    }
}
