using System;
using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class FissionCommand : ICommand
    {
        public readonly Vector NearDistance;
        public readonly int M;

        public FissionCommand(Vector nearDistance, int m)
        {
            NearDistance = nearDistance;
            M = m;
        }
        public void Apply(State state, Bot bot)
        {
            Array.Sort(bot.Seeds);

            var transferedSeeds = bot.Seeds.Skip(1).Take(M).ToArray();
            var newSeeds = bot.Seeds.Skip(M + 1).ToArray();

            var newBot = new Bot(bot.Seeds[0], bot.Pos + NearDistance, transferedSeeds);
            bot.Seeds = newSeeds;

            state.Bots = state.Bots.Concat(new[] {newBot}).OrderBy(b => b.Bid).ToArray();
            state.Energy += 24;

            state.OpLogWriter.WriteAdd(newBot.Pos);
            state.OpLogWriter.WriteEnergy(state.Energy);
        }

	    public ICommand Revert()
	    {
		    throw new NotImplementedException();
	    }
    }
}
