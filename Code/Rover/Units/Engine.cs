using System;
using WALLE.Rover.Dto.Commands;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover.Units
{
    internal class Engine
    {
        private readonly EngineData _engineData;

        public Engine(TelemetryData telemetryData)
        {
            _engineData = new EngineData();

            telemetryData.EngineData.Add(_engineData);
        }

        public void SetEngine(MoveDirection direction, TimeSpan runtime)
        {
            _engineData.Direction = direction;
            _engineData.RunTime = runtime;
        }
    }
}
