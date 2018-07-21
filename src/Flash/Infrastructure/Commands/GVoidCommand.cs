using System.Collections.Generic;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class GVoidCommand : IGroupCommand
    {
        public Vector NearDistance { get; }
        public Vector FarDistance { get; }

        public GVoidCommand(Vector nearDistance, Vector farDistance)
        {
            NearDistance = nearDistance;
            FarDistance = farDistance;
        }
        public void Apply(State state, Bot bot)
        {
            var region = new Region(bot.Pos + NearDistance, bot.Pos + NearDistance + FarDistance);

            var removed = new List<Vector>();
            for (var x = region.Min.X; x <= region.Max.X; x++)
            {
                for (var y = region.Min.Y; y <= region.Max.Y; y++)
                {
                    for (var z = region.Min.Z; z <= region.Max.Z; z++)
                    {
                        var vector = new Vector(x, y, z);
                        if (state.Matrix.IsFull(vector))
                        {
                            removed.Add(vector);
                            state.Energy -= 12;
                            state.Matrix.Clear(vector);
                        }
                        else
                        {
                            state.Energy += 3;
                        }

                    }
                }
            }

            state.OpLogWriter.WriteGroupRemove(removed.ToArray());
        }
    }
}
