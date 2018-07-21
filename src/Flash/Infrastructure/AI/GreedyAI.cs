using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure
{
	public class GreedyAI : IAI
	{
		public IEnumerable<ICommand> NextStep(State state)
		{
			var myBot = state.Bots.First();

			var firstOrDefault = myBot.Pos.GetNears().FirstOrDefault(x => x.IsGood(state.Matrix.R) && state.Matrix.IsFull(x) && LifeWillGoodWithoutMe(x, state));

			if (firstOrDefault != null)
			{
				yield return new ClearCommand(firstOrDefault - myBot.Pos);
			}
			else
			{
				var vector = Bfs(myBot, state);

				if (vector == null)
				{
					if (Equals(myBot.Pos, new Vector(0, 0, 0)))
					{
						yield return new HaltCommand();
					}
					else
					{
						var a = Bfs1(myBot, state);
						yield return new SMoveCommand(a - myBot.Pos);
					}
				}
				else
					yield return new SMoveCommand(vector - myBot.Pos);
			}
		}

		public static Vector Bfs(Bot bot, State state)
		{
			var parents = new Dictionary<Vector, Vector>();
			var visited = new HashSet<Vector>();
			var queue = new Queue<Vector>();
			queue.Enqueue(bot.Pos);

			while (queue.Any())
			{
				var cur = queue.Dequeue();
				for (var i = 0; i < 6; i++)
				{
					for (var n = 1; n < 15; n++)
					{
						var adj = cur.GetAdjacent(i, n);
						if (!adj.IsGood(state.Matrix.R) || state.Matrix.IsFull(adj))
							break;

						if (!visited.Contains(adj))
						{
							parents.Add(adj, cur);
							queue.Enqueue(adj);
							visited.Add(adj);

							var flag = false;

							foreach (var near in adj.GetNears().Where(x => x.IsGood(state.Matrix.R) && state.Matrix.IsFull(x)))
							{
								flag = LifeWillGoodWithoutMe(near, state);

								if(flag)
									break;
							}

							if (flag)
							{
								var curr = adj;
								while (!Equals(parents[curr], bot.Pos))
								{
									curr = parents[curr];
								}

								return curr;
							}
						}
					}
				}
			}

			return null;
		}


		public static Vector Bfs1(Bot bot, State state)
		{
			var parents = new Dictionary<Vector, Vector>();
			var visited = new HashSet<Vector>();
			var queue = new Queue<Vector>();
			queue.Enqueue(bot.Pos);

			while (queue.Any())
			{
				var cur = queue.Dequeue();
				for (var i = 0; i < 6; i++)
				{
					for (var n = 1; n < 15; n++)
					{
						var adj = cur.GetAdjacent(i, n);
						if (!adj.IsGood(state.Matrix.R) || state.Matrix.IsFull(adj))
							break;

						if (!visited.Contains(adj))
						{
							parents.Add(adj, cur);
							queue.Enqueue(adj);
							visited.Add(adj);

							if (Equals(adj, new Vector(0, 0, 0)))
							{
								var curr = adj;
								while (!Equals(parents[curr], bot.Pos))
								{
									curr = parents[curr];
								}

								return curr;
							}
						}
					}
				}
			}

			return null;
		}

		public static bool LifeWillGoodWithoutMe(Vector me, State state)
		{
			state.Matrix.Clear(me);
			var flag = me.GetAdjacents().Where(x => x.IsGood(state.Matrix.R)).All(x => state.Matrix.IsGrounded(x));
			state.Matrix.Fill(me);

			return flag;
		}
	}
}