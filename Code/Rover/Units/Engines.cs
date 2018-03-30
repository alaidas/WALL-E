using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WALLE.Rover.Dto.Commands;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover.Units
{
    internal class Engines
    {
        private readonly EngineData _leftEngine;
        private readonly EngineData _rightEngine;

        public Engines(TelemetryData telemetryData)
        {
            _leftEngine = telemetryData.LeftEngine;
            _rightEngine = telemetryData.RightEngine;
        }

        public void SetLeftEngineData(MoveDirection direction, int runTimeMiliseconds)
        {
            _leftEngine.Direction = direction;
            _leftEngine.RunTime = runTimeMiliseconds;
        }

        public void SetRightEngineData(MoveDirection direction, int runTimeMiliseconds)
        {
            _rightEngine.Direction = direction;
            _rightEngine.RunTime = runTimeMiliseconds;
        }
    }
}
