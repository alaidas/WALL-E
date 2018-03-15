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

        public Engines Engines => new Engines(_telemetryData);
    }
}
