using WALLE.Rover.Dto.Commands;

namespace WALLE.Rover.Dto.Telemetry
{
    internal class EngineData
    {
        public MoveDirection Direction { get; set; }

        public int RunTime { get; set; }
    }
}
