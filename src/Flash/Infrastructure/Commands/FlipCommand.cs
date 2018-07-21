using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class FlipCommand: ICommand
    {
        public void Apply(State state, Bot bot)
        {
            state.Harmonics = !state.Harmonics;

            state.OpLogWriter.WriteHarmonic(state.Harmonics);
        }
    }
}