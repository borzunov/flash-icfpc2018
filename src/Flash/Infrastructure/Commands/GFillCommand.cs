using System;
using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class GFillCommand : ICommand
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
            throw new NotImplementedException();
        }
    }
}
