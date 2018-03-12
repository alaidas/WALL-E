using System;
using Microsoft.Extensions.DependencyInjection;
using WALLE.Link.Logging;

namespace WALLE.Link.Extensions
{
    public static class LinkServiceCollectionExtensions
    {
        public static void AddHttpLink(this IServiceCollection services)
        {
            services.AddSingleton<ILinkClient, LinkClientHttp>();
        }

        public static void AddAmqpLink(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public static void AddLinkLogger(this IServiceCollection services)
        {
            services.AddSingleton<LinkLoggerConfiguration>();
            services.AddSingleton<LinkLoggerProvider>();
        }
    }
}
