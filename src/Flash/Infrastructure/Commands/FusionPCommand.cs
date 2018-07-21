using System;

namespace Flash.Infrastructure.Commands
{
    public class FusionPCommand : ICommand
    {
        public readonly Vector NearDistance;

        public FusionPCommand(Vector nearDistance)
        {
            this.NearDistance = nearDistance;
        }
        public void Apply(State state, Bot bot)
        {
            throw new NotImplementedException();
        }
    }
}
