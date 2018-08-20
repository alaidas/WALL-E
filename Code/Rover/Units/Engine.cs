using System;
using WALLE.Rover.Dto.Commands;
using WALLE.Rover.Dto.Telemetry;

namespace WALLE.Rover.Units
{
    internal class Engine
    {
        public readonly EngineData EengineData = new EngineData();

        public void SetEngine(MoveDirection direction, TimeSpan runtime)
        {
            EengineData.Direction = direction;
            EengineData.RunTime = runtime;
        }
    }
}
