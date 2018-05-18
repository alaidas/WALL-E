namespace WALLE.Rover.Dto.Commands
{
    internal enum MoveDirection
    {
        None = 0,
        Forward = 1,
        Backward = 2
    }

    internal class MoveCommand
    {
        public MoveDirection Direction { get; set; }

        public int TimeInMiliseconds { get; set; }
    }
}
