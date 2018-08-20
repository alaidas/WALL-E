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
using WALLE.Rover.Units;

namespace WALLE.Rover
{
    internal class CommandsProcessor : BackgroundService
    {
        private readonly ILinkClient _linkClient;
        private readonly ILogger<CommandsProcessor> _logger;
        private readonly UnitsController _unitsController;
        private CancellationToken _shutdownToken;

        public CommandsProcessor(ILinkClient linkClient, UnitsController unitsController, ILogger<CommandsProcessor> logger)
        {
            _linkClient = linkClient;
            _unitsController = unitsController;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _shutdownToken = cancellationToken;

            using (_linkClient.SubscribeForCommands(ProcessCommandAsync))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(10, cancellationToken);
                }
            }
        }

        private async void ProcessCommandAsync(Event @event)
        {
            try
            {
                Console.WriteLine($"Command is reveived: {JsonConvert.SerializeObject(@event)}");

                string json = @event.Content;
                switch (@event.ContentType)
                {
                    case nameof(MoveCommand):
                        await ProcessMoveAsync(JsonConvert.DeserializeObject<MoveCommand>(json), _shutdownToken);
                        break;

                    case nameof(TurnCommand):
                        await ProcessTurnAsync(JsonConvert.DeserializeObject<TurnCommand>(json), _shutdownToken);
                        break;

                    default:
                        throw new NotSupportedException($"Not supported command: '{@event.ContentType}'");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task ProcessMoveAsync(MoveCommand moveCommand, CancellationToken cancellationToken)
        {
            TimeSpan runtime = TimeSpan.FromMilliseconds(moveCommand.TimeInMiliseconds);

            switch (moveCommand.Direction)
            {
                case MoveDirection.Forward:
                case MoveDirection.Backward:
                    {
                        _unitsController.WheelsEngine[0].SetEngine(moveCommand.Direction, runtime);
                        _unitsController.WheelsEngine[1].SetEngine(moveCommand.Direction, runtime);

                        await Task.Delay(runtime, cancellationToken);

                        _unitsController.WheelsEngine[0].SetEngine(MoveDirection.None, TimeSpan.Zero);
                        _unitsController.WheelsEngine[1].SetEngine(MoveDirection.None, TimeSpan.Zero);

                        break;
                    }

                default:
                    throw new NotSupportedException($"Not supported direction: '{moveCommand.Direction}'");
            }
        }

        private async Task ProcessTurnAsync(TurnCommand turnCommand, CancellationToken cancellationToken)
        {
            if (turnCommand.Angle <= 0 || turnCommand.Angle > 360) return;

            TimeSpan runtime = TimeSpan.FromMilliseconds((double)(Decimal.Divide(14 * turnCommand.Angle, 2)));

            MoveDirection leftEngine;
            MoveDirection rigthEngine;

            switch (turnCommand.Direction)
            {
                case TurnDirection.TrunLeft:
                    leftEngine = MoveDirection.Backward;
                    rigthEngine = MoveDirection.Forward;
                    break;

                case TurnDirection.TurnRight:
                    leftEngine = MoveDirection.Forward;
                    rigthEngine = MoveDirection.Backward;
                    break;

                default:
                    throw new NotSupportedException($"Not supported direction: '{turnCommand.Direction}'");
            }

            _unitsController.WheelsEngine[0].SetEngine(leftEngine, runtime);
            _unitsController.WheelsEngine[1].SetEngine(rigthEngine, runtime);

            await Task.Delay(runtime, cancellationToken);

            _unitsController.WheelsEngine[0].SetEngine(MoveDirection.None, TimeSpan.Zero);
            _unitsController.WheelsEngine[1].SetEngine(MoveDirection.None, TimeSpan.Zero);
        }
    }
}
