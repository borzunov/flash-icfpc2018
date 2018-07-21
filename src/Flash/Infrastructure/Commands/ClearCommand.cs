using System;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class ClearCommand : ICommand
    {
        public readonly Vector NearDistance;

        public ClearCommand(Vector nearDistance)
        {
            NearDistance = nearDistance;
        }

        public void Apply(State state, Bot bot)
        {
            var voxel = bot.Pos + NearDistance;

            if (!state.Matrix.IsFull(voxel))
            {
                state.Energy += 6;
            }
            else
            {
                state.Energy += 12;
                state.Matrix.Clear(voxel);
            }

           // state.OpLogWriter.WriteFill(voxel);
           // state.OpLogWriter.WriteEnergy(state.Energy);
        }

	    public ICommand Revert()
	    {
		    return new FillCommand(NearDistance);
	    }
    }
}
