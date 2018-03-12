using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WALLE.Link.Logging
{
    internal class LinkLoggerProvider: ILoggerProvider
    {
        private readonly LinkLoggerConfiguration _configuration;
        private readonly ILinkClient _linkClient;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        public LinkLoggerProvider(ILinkClient linkClient, LinkLoggerConfiguration linkLoggerConfiguration)
        {
            _configuration = linkLoggerConfiguration;
            _linkClient = linkClient;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new LinkLogger(name, _configuration, _linkClient));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
