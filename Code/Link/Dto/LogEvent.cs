using Microsoft.Extensions.Logging;

namespace WALLE.Link.Dto
{
    internal class LogEvent
    {
        public LogLevel LogLevel { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }
    }
}
