namespace Flash.Infrastructure.Commands
{
    public class FlipCommand: ICommand
    {
        public void Apply(State state, Bot bot)
        {
            state.Harmonics = !state.Harmonics;
        }
    }
}