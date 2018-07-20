namespace Flash.Infrastructure.Commands
{
    public class SMoveCommand : ICommand
    {
        public Vector Direction { get; }

        public SMoveCommand(Vector direction)
        {
            Direction = direction;
        }
    }
}
