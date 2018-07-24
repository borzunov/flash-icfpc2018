using System;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class FillCommand : ICommand
	{
		public readonly Vector NearDistance;
		public readonly Vector RealFill;

		public FillCommand(Vector nearDistance, Vector realFill = null)
        {
            NearDistance = nearDistance;
	        RealFill = realFill;
        }

        public void Apply(State state, Bot bot)
        {
            var voxel = bot.Pos + NearDistance;

            if (state.Matrix.IsFull(voxel))
            {
                state.Energy += 6;
            }
            else
            {
				if(voxel == new Vector(16, 1, 9))
					Console.WriteLine();
                state.Energy += 12;
                state.Matrix.Fill(voxel);
            }

            state.OpLogWriter.WriteFill(voxel);
            state.OpLogWriter.WriteEnergy(state.Energy);
        }
    }
}
