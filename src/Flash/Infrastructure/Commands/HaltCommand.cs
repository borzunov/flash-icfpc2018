using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class HaltCommand : ICommand
    {
        public void Apply(State state, Bot bot)
        {
            state.Bots = new Bot[0];

            state.OpLogWriter.WriteRemove(bot.Pos);
            state.OpLogWriter.WriteMessage("Game over!");
        }

	    public ICommand Revert()
	    {
		    throw new System.NotImplementedException();
	    }
    }
}