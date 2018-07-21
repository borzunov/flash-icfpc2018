using Flash.Infrastructure.Models;

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
            var oldPosition = bot.Pos;
            var newPosition = bot.Pos + Direction;
            
            bot.Pos = newPosition;
            state.Energy += 2*Direction.Mlen;

            state.OpLogWriter.WriteRemove(oldPosition);
            state.OpLogWriter.WriteAdd(newPosition);
        }
    }
}
