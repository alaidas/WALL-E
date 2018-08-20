using System.Collections.Generic;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover.Units
{
    internal class UnitsController
    {
        private readonly TelemetryData _telemetryData;

        public readonly Engine[] WheelsEngine = new [] { new Engine(), new Engine() };

        public UnitsController(TelemetryData telemetryData)
        {
            _telemetryData = telemetryData;

            _telemetryData.EngineData.Add(WheelsEngine[0].EengineData);
            _telemetryData.EngineData.Add(WheelsEngine[1].EengineData);
        }
    }
}
