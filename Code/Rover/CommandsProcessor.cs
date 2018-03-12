using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link;
using WALLE.Link.Dto;
using WALLE.Rover.Dto;
using WALLE.Rover.Dto.Commands;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover
{
    internal class CommandsProcessor : BackgroundService
    {
        private readonly ILinkClient _linkClient;
        private readonly ILogger<CommandsProcessor> _logger;

        public CommandsProcessor(ILinkClient linkClient, ILogger<CommandsProcessor> logger)
        {
            _linkClient = linkClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (_linkClient.SubscribeForCommands(ProcessCommand))
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10, stoppingToken);
                }
            }
        }

        private void ProcessCommand(Event @event)
        {
            try
            {
                Console.WriteLine($"Command is reveived: {JsonConvert.SerializeObject(@event)}");

                string json = Encoding.UTF8.GetString(@event.Content);
                switch (@event.ContentType)
                {
                    case nameof(MoveCommand):
                        ProcessMove(JsonConvert.DeserializeObject<MoveCommand>(json));
                        break;

                    default:
                        throw new NotSupportedException($"Not supported command: '{@event.ContentType}'");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void ProcessMove(MoveCommand engineData)
        {
            switch(engineData.Direction)
            {
                case MoveDirection.Forward:
                    break;
                case MoveDirection.Backward:
                    break;
                case MoveDirection.TrunLeft:
                    break;
                case MoveDirection.TurnRight:
                    break;
            }
        }
    }
}
