namespace Flash.Infrastructure.Commands
{
    public class HaltCommand : ICommand
    {
        public void Apply(State state, Bot bot)
        {
            state.Bots = new Bot[0];
        }
    }
}