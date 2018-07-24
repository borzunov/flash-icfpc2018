using System;
using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class VoidCommand : ICommand
    {
        public readonly Vector NearDistance;
        public readonly Vector RealVoid;

        public VoidCommand(Vector nearDistance, Vector realVoid = null)
        {
            NearDistance = nearDistance;
            RealVoid = realVoid;
        }

        public void Apply(State state, Bot bot)
        {
			
            var vector = bot.Pos + NearDistance;
            if (state.Matrix.IsFull(vector))
            {
                state.OpLogWriter.WriteRemove(vector);
                state.Matrix.Clear(vector);
	            if (vector == new Vector(16, 1, 9))
		            Console.WriteLine();
					state.Energy -= 12;
            }
            else
            {
                state.Energy += 3;
            }
        }
    }
}
