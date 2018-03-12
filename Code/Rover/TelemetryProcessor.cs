using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link;
using WALLE.Rover.Dto;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover
{
    internal class TelemetryProcessor : BackgroundService
    {
        private readonly ILogger<TelemetryProcessor> _logger;
        private readonly ILinkClient _linkClient;
        private readonly TelemetryData _currentTelemetry;

        public TelemetryProcessor(TelemetryData telemetry, ILinkClient linkClient, ILogger<TelemetryProcessor> logger)
        {
            _currentTelemetry = telemetry;
            _linkClient = linkClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await SendTelemetryAsync(cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task SendTelemetryAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _linkClient.SendEventAsync(new Link.Dto.Event
                {
                    Id = Guid.NewGuid().ToString(),
                    CreationTime = DateTime.UtcNow,
                    Sender = "WALL-E Rover",
                    ContentType = nameof(TelemetryData),
                    Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_currentTelemetry))
                }, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
