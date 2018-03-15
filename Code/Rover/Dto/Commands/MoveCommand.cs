namespace WALLE.Rover.Dto.Commands
{
    internal enum MoveDirection
    {
        Forward = 0,
        Backward = 1
    }

    internal class MoveCommand
    {
        public MoveDirection Direction { get; set; }

        public int TimeInMiliseconds { get; set; }
    }
}
