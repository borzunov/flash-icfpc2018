namespace Flash.Infrastructure.Commands
{
    public interface ICommand
    {
        void Apply(State state, Bot bot);
    }
}