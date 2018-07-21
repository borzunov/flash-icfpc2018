namespace Flash.Infrastructure.Commands
{
    public class FusionSCommand : ICommand
    {
        public readonly Vector NearDistance;

        public FusionSCommand(Vector nearDistance)
        {
            this.NearDistance = nearDistance;
        }
        public void Apply(State state, Bot bot)
        {
        }
    }
}