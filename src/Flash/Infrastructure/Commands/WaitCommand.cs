using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class WaitCommand : ICommand
    {
        public void Apply(State state, Bot bot)
        {
        }

	    public ICommand Revert()
	    {
		    return this;
	    }
    }
}
