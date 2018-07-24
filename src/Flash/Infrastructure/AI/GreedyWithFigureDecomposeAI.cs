using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Alghorithms;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.AI
{
	public class GreedyWithFigureDecomposeAI : IAI
	{
		private readonly IsGroundedChecker groundedChecker;
		private readonly Matrix matrix;
		private readonly Queue<BuildingTask> buildingTasks;
		private Dictionary<int, Queue<ICommand>> commandsQueue;
		private Queue<ICommand> myQueue;
		private int myBodBid = 0;
		private Queue<ICommand> fussionQueue;
		private bool FLAG = true;
		public HashSet<Vector> freeBots;
		public Dictionary<BuildingTask, HashSet<Bot>> taskToBots;		

		public HashSet<Vector> tehdolg = new HashSet<Vector>();

		public GreedyWithFigureDecomposeAI(List<BuildingTask> buildingTasks, IsGroundedChecker groundedChecker, Matrix matrix)
		{
			this.groundedChecker = groundedChecker;
			this.matrix = matrix;
			this.buildingTasks = new Queue<BuildingTask>(buildingTasks.Where(x => x.Type == BuildingTaskType.Fill || x.Type == BuildingTaskType.GFill));
		}

		public IEnumerable<ICommand> NextStep(State state)
		{
			if (FLAG)
			{
				FLAG = false;
				return new List<ICommand> { new FlipCommand() };
			}
			var ans = new List<ICommand>();

			var firstOrDefault = state.Bots.FirstOrDefault(x => state.Matrix.Contains(x.Pos) && state.Matrix.IsFull(x.Pos));
			if (firstOrDefault != null || state.Bots.Any(x => state.Bots.Where(y => y.Bid != x.Bid).Any(z => z.Pos == x.Pos)))
			{
				Console.WriteLine();

				return new List<ICommand> {new HaltCommand()};
				return ans;
			}

			//самоуничтожение
			if (buildingTasks.Count == 0)
			{
				if (state.Bots.Length == 1)
				{
					if (state.Bots.Single().Pos != new Vector(0, 0, 0))
					{
						fussionQueue = fussionQueue ??
									   new Queue<ICommand>(GetMoveCommands(state, state.Bots.Single(), new Vector(0, 0, 0),
										   new HashSet<Vector>()).commands);

						ans.Add(fussionQueue.Dequeue());
					}
					else
					{
						ans.Add(new FlipCommand());
						ans.Add(new HaltCommand());
						return ans;
					}
				}

				for (var index = 0; index < state.Bots.Length - 1; index++)
				{
					if (index < state.Bots.Length - 2)
					{
						ans.Add(new WaitCommand());
						continue;
					}

					var bot1 = state.Bots[index];
					var bot2 = state.Bots[index + 1];

					var near = bot1.Pos.GetNears().FirstOrDefault(x => x == bot2.Pos);
					if (near != null && fussionQueue?.Count == 0)
					{
						ans.Add(new FusionPCommand(near - bot1.Pos));
						ans.Add(new FusionSCommand(new Vector(0, 0, 0) - near + bot1.Pos));

						if (matrix.IsFull(bot2.Pos))
							tehdolg.Add(bot2.Pos);
						fussionQueue = null;
					}
					else
					{
						ans.Add(new WaitCommand());

						var nearest = GetNearestTargetPoint(bot2.Pos, bot1.Pos, v => state.Matrix.Contains(v) && state.Bots.All(x => x.Pos != v));
						fussionQueue = fussionQueue ?? new Queue<ICommand>(GetMoveCommands(state, bot2, nearest, new HashSet<Vector>()).commands);

						var command = fussionQueue.Dequeue();

						ans.Add(command);
					}
				}
			}
			else if (state.Bots.Length != 8)
			{
				ans.AddRange(BotDivider.GetDivideCommandsForOneStep(state, state.Bots.Length));
			}
			else
			{
				var task = buildingTasks.Peek();
		
				var targetPoints = task.Region.Points();

				commandsQueue = commandsQueue ?? GetCommadsQueue(state, targetPoints, task.Region, true);

				if (!commandsQueue.Any() || commandsQueue.All(x => x.Value.Count == 0))
					commandsQueue = GetCommadsQueue(state, targetPoints, task.Region, true);

				if (!commandsQueue.Any() || commandsQueue.All(x => x.Value.Count == 0))
				{
					foreach (var fillCommand in GetFillCommands(state, task))
					{
						ans.Add(fillCommand);
					}

					commandsQueue = null;
					buildingTasks.Dequeue();
				}
				else
				{
					foreach (var bot in state.Bots)
					{
						if (commandsQueue.ContainsKey(bot.Bid) && commandsQueue[bot.Bid].Count != 0)
						{
							var moveCommand = commandsQueue[bot.Bid].Dequeue();

							ans.Add(moveCommand);
						}
						else
						{
							ans.Add(new WaitCommand());
						}
					}
				}
			}

			if (ans.Count != state.Bots.Length)
				throw new InvalidOperationException();

			return ans;
		}


		private Dictionary<int, Queue<ICommand>> GetCommadsQueue(State state, List<Vector> points, Region region, bool flag)
		{
			var commands = new Dictionary<int, Queue<ICommand>>();
			var fobidden = new HashSet<Vector>();
			var used = new HashSet<Vector>();
			for (var index = 0; index < points.Count; index++)
			{
				var bot = state.Bots[index];
				var target = points[index];
				if ((bot.Pos - target).IsNd)
					continue;

				var point = GetNearestTargetPoint(bot.Pos, target,
					v => state.Matrix.Contains(v)
					&& !region.Contains(v)
					&& state.Bots.All(x => bot.Bid == x.Bid || x.Pos != v)
					&& !used.Contains(v)
					&& !fobidden.Contains(v));

				if (point == null)
					continue;

				used.Add(point);
				
				fobidden.UnionWith(used);
				var (positions, moveCommands) = GetMoveCommands(state, bot, point, fobidden, flag);
				if (positions.Any())
					fobidden.UnionWith(positions);

				if (moveCommands.Any())
					commands.Add(bot.Bid, new Queue<ICommand>(moveCommands));
			}

			for (var i = points.Count; i < state.Bots.Length; i++)
			{
				var bot = state.Bots[i];
				if (region.Contains(bot.Pos))
				{
					var point = new Vector(0, 0, i);

					used.Add(point);
					fobidden.UnionWith(used);
					var (positions, moveCommands) = GetMoveCommands(state, bot, point, fobidden, flag);
					fobidden.UnionWith(positions);

					if (moveCommands.Any())
						commands.Add(bot.Bid, new Queue<ICommand>(moveCommands));
				}
			}

			return commands;
		}

		private Vector GetNearestTargetPoint(Vector me, Vector target, Func<Vector, bool> isGoodPoint)
		{
			return target
				.GetNears()
				.Where(isGoodPoint)
				.OrderBy(x => (x - me).Mlen)
				.FirstOrDefault();
		}

		private IEnumerable<ICommand> GetFillCommands(State state, BuildingTask task)
		{
			var points = task.Region.Points();
			var revPoints = task.Region.RevPoints();
			var activeBotsCount = task.Region.Points().Count;

			switch (task.Type)
			{
				case BuildingTaskType.GFill:
					for (var index = 0; index < activeBotsCount; index++)
					{
						var bot = state.Bots[index];
						var point = points[index];
						var revPoint = revPoints[index];
						var gFillCommand = new GFillCommand(point - bot.Pos, revPoint - point);
						yield return gFillCommand;
					}
					break;
				case BuildingTaskType.GVoid:
					for (var index = 0; index < activeBotsCount; index++)
					{
						var bot = state.Bots[index];
						var point = points[index];
						var revPoint = revPoints[index];
						var gFillCommand = new GVoidCommand(point - bot.Pos, revPoint - point);
						yield return gFillCommand;
					}
					break;
				case BuildingTaskType.Fill:
					yield return new FillCommand(points[0] - state.Bots.First().Pos);
					break;
				case BuildingTaskType.Void:
					yield return new VoidCommand(points[0] - state.Bots.First().Pos);
					break;
				default:
					throw new InvalidOperationException();
			}


			for (var i = activeBotsCount; i < state.Bots.Length; i++)
			{
				yield return new WaitCommand();
			}
		}


		private IEnumerable<ICommand> GetFirstCommads(State state)
		{
			for (var i = 0; i < state.Bots.Length - 1; i++)
			{
				yield return new WaitCommand();
			}

			var bot = state.Bots.Last();
			yield return new FissionCommand(new Vector(1, 0, 0), bot.Seeds.Length - 1);
		}

		private (List<Vector> positions, List<ICommand> commands) GetMoveCommands(State state, Bot bot, Vector target,
			HashSet<Vector> fobidden, bool flag = true)
		{
			if (bot.Pos == target)
				return (new List<Vector>(), new List<ICommand>());

			bool IsForbiddenArea(Vector v) => fobidden.Contains(v) || state.Bots.Any(x => x.Bid != bot.Bid && x.Pos == v);
			var botMoveSearcher = new BotMoveSearcher(state.Matrix, matrix, bot.Pos, IsForbiddenArea, state.Bots.Length, target,
				groundedChecker);

			var s = botMoveSearcher.FindPath(out var positions, out var commands, out _);

			if (!s)
			{
				return (new List<Vector>(), new List<ICommand>());
			}

			var enumerable = tehdolg.Where(x => bot.Pos.GetNears().Contains(x)).ToList();
			tehdolg.ExceptWith(enumerable);
			var clearCommand = enumerable.Select(x => new FillCommand(x - bot.Pos)).ToList();
			commands = clearCommand.Concat(commands).ToList();
			if(bot.Bid == 3)
				Console.WriteLine();

			if(commands.Any(x => x is VoidCommand && (x as VoidCommand).RealVoid == new Vector(16, 1, 8)))
				Console.WriteLine();
			return (positions, commands);
		}
	}
}