namespace Flash.Infrastructure.Commands
{
    public class LMoveCommand : ICommand
    {
        public Vector FirstDirection { get; }
        public Vector SecondDirection { get; }

        public LMoveCommand(Vector firstDirection, Vector secondDirection)
        {
            FirstDirection = firstDirection;
            SecondDirection = secondDirection;
        }
    }
}
