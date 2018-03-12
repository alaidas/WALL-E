namespace WALLE.Rover.Dto.Commands
{
    internal enum MoveDirection
    {
        Forward = 0,
        Backward = 1,
        TrunLeft = 2,
        TurnRight = 3
    }

    internal class MoveCommand
    {
        public MoveDirection Direction { get; set; }

        public int TimeInMiliseconds { get; set; }
    }
}
