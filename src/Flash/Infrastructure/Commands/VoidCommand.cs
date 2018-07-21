using System;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class VoidCommand : ICommand
    {
        public readonly Vector NearDistance;

        public VoidCommand(Vector nearDistance)
        {
            NearDistance = nearDistance;
        }

        public void Apply(State state, Bot bot)
        {
            var vector = bot.Pos + NearDistance;
            if (state.Matrix.IsFull(vector))
            {
                state.OpLogWriter.WriteRemove(vector);
                state.Matrix.Fill(vector);
                state.Energy -= 12;
            }
            else
            {
                state.Energy += 3;
            }
        }

        public ICommand Revert()
        {
            throw new NotImplementedException();
        }
    }
}
