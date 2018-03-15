namespace WALLE.Rover.Dto.Commands
{
    internal enum TurnDirection
    {
        TrunLeft = 1,
        TurnRight = 2
    }

    internal class TurnCommand
    {
        public TurnDirection Direction { get; set; }

        public int Angle { get; set; }
    }
}
