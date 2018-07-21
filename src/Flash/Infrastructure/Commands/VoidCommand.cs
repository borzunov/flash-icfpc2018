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
            throw new NotImplementedException();
        }
    }
}
