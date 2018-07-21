using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class GFillCommand : IGroupCommand
    {
        public Vector NearDistance { get; }
        public Vector FarDistance { get; }

        public GFillCommand(Vector nearDistance, Vector farDistance)
        {
            NearDistance = nearDistance;
            FarDistance = farDistance;
        }
        public void Apply(State state, Bot bot)
        {
            var region = new Region(bot.Pos + NearDistance, bot.Pos + NearDistance + FarDistance);
            var added = new List<Vector>();

            for (var x = region.Min.X; x <= region.Max.X; x++)
            {
                for (var y = region.Min.Y; y <= region.Max.Y; y++)
                {
                    for (var z = region.Min.Z; z <= region.Max.Z; z++)
                    {
                        var vector = new Vector(x, y, z);
                        if (state.Matrix.IsVoid(vector))
                        {
                            state.Matrix.Fill(vector);
                            state.Energy += 12;
                            added.Add(vector);
                        }
                        else
                        {
                            state.Energy += 6;
                        }

                    }
                }
            }

            state.OpLogWriter.WriteGroupAdd(added.ToArray());
        }

        public ICommand Revert()
        {
            throw new NotImplementedException();
        }
    }
}
