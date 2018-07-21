using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public class FusionPCommand : ICommand
    {
        public readonly Vector NearDistance;

        public FusionPCommand(Vector nearDistance)
        {
            NearDistance = nearDistance;
        }
        public void Apply(State state, Bot bot)
        {
            var secondaryBot = state.Bots.First(b => Equals(b.Pos, bot.Pos + NearDistance));
            var newSeeds = bot.Seeds.Concat(secondaryBot.Seeds).Concat(new[] {bot.Bid});
            bot.Seeds = newSeeds.OrderBy(s => s).ToArray();

            state.Bots = state.Bots.Where(b => b.Bid != secondaryBot.Bid).ToArray();
            state.Energy -= 24;

            state.OpLogWriter.WriteRemove(secondaryBot.Pos);
            state.OpLogWriter.WriteEnergy(state.Energy);
        }
    }
}
