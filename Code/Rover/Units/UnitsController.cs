using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover.Units
{
    internal class UnitsController
    {
        private readonly TelemetryData _telemetryData;

        public UnitsController(TelemetryData telemetryData)
        {
            _telemetryData = telemetryData;
        }

        public Engine[] Engines => new[] { new Engine(_telemetryData), new Engine(_telemetryData) };
    }
}
