using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Alghorithms
{
	public static class BotDivider
	{
		public static IEnumerable<ICommand> GetDivideCommads(State state, int n)
		{
			var commands = new List<ICommand>();
			var currentBotCount = 1;

			while (currentBotCount < n)
			{
				commands.AddRange(GetDivideCommandsForOneStep(state, currentBotCount));
				currentBotCount++;
			}

			return commands;
		}

		public static IEnumerable<ICommand> GetDivideCommandsForOneStep(State state, int currentBotCount)
		{
			for (var i = 0; i < currentBotCount - 1; i++)
			{
				yield return new WaitCommand();
			}

			var bot = state.Bots.Last();

			var nears = new List<Vector> { new Vector(1, 0, 0), new Vector(0, 1, 0), new Vector(0, 0, 1) };

			var near = nears.FirstOrDefault(n => state.Matrix.Contains(bot.Pos + n));

			if (near == null)
				throw new InvalidOperationException();

			yield return new FissionCommand(near, bot.Seeds.Length - 1);
		}
	}
}