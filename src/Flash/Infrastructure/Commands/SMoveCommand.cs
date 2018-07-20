namespace Flash.Infrastructure.Commands
{
    public class SMoveCommand : ICommand
    {
        public Vector Direction { get; }

        public SMoveCommand(Vector direction)
        {
            Direction = direction;
        }

        public void Apply(State state, Bot bot)
        {
            var newPositon = bot.Pos + Direction;
            
            bot.Pos = newPositon;
            state.Energy += 2*Direction.Mlen;
        }
    }
}
