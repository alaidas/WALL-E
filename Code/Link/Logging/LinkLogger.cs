using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link.Dto;

namespace WALLE.Link.Logging
{
    internal class LinkLogger : ILogger
    {
        private readonly LinkLoggerConfiguration _configuration;
        private readonly ILinkClient _linkClient;
        private readonly string _name;

        public LinkLogger(string name, LinkLoggerConfiguration configuration, ILinkClient linkClient)
        {
            _configuration = configuration;
            _linkClient = linkClient;
            _name = name;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            _linkClient.SendEventAsync(new Dto.Event
            {
                ContentType = nameof(LogEvent),
                Sender = _configuration.Sender,
                CreationTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Content = GetContent(logLevel, state, exception, formatter)
            }, CancellationToken.None).ContinueWith(result =>
            {
                if (result.Exception != null)
                    Console.WriteLine(result.Exception.ToString());
            });
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _configuration.LogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private string GetContent<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string json = JsonConvert.SerializeObject(new LogEvent
            {
                LogLevel = logLevel,
                Name = _name,
                Message = formatter(state, exception)
            });

            return json;
        }
    }
}
