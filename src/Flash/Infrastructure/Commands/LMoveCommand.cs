using Flash.Infrastructure.Models;

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

        public void Apply(State state, Bot bot)
        {
            var oldPosition = bot.Pos;
            var newPosition = bot.Pos + FirstDirection + SecondDirection;

            bot.Pos = newPosition;
            state.Energy += 2*(FirstDirection.Mlen + 2 + SecondDirection.Mlen);

            state.OpLogWriter.WriteRemove(oldPosition);
            state.OpLogWriter.WriteAdd(newPosition);
            state.OpLogWriter.WriteEnergy(state.Energy);
        }
    }
}
