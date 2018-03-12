using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WALLE.Link.Logging
{
    internal class LinkLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; }

        public string Sender { get; set; }

        public LinkLoggerConfiguration(IConfiguration configuration)
        {
            IConfigurationSection config = configuration.GetSection("Logging").GetSection("Link");

            LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), config[nameof(LogLevel)], true);
            Sender = config[nameof(Sender)];
        }
    }
}
