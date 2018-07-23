using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.AI
{
	public class GreedyWithFigureDecomposeAI : IAI
	{
		private readonly IsGroundedChecker groundedChecker;
		private readonly Queue<BuildingTask> buildingTasks;
		private Dictionary<int, Queue<ICommand>> commandsQueue;
		private Queue<ICommand> myQueue;
		private int myBodBid = 0;
		private Queue<ICommand> fussionQueue;
		private bool FLAG = true;

		public GreedyWithFigureDecomposeAI(List<BuildingTask> buildingTasks, IsGroundedChecker groundedChecker)
		{
			this.groundedChecker = groundedChecker;
			this.buildingTasks = new Queue<BuildingTask>(buildingTasks);
		}

		public IEnumerable<ICommand> NextStep(State state)
		{
			if (FLAG)
			{
				FLAG = false;
				return new List<ICommand> {new FlipCommand()};
			}
			var ans = new List<ICommand>();

			var firstOrDefault = state.Bots.FirstOrDefault(x => state.Matrix.Contains(x.Pos) && state.Matrix.IsFull(x.Pos));
			if (firstOrDefault != null || state.Bots.Any(x => state.Bots.Where(y => y.Bid != x.Bid).Any(z => z.Pos == x.Pos)))
			{
				Console.WriteLine();

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
					if (near != null)
					{
						ans.Add(new FusionPCommand(near - bot1.Pos));
						ans.Add(new FusionSCommand(new Vector(0, 0, 0) - near + bot1.Pos));
						fussionQueue = null;
					}
					else
					{
						ans.Add(new WaitCommand());

						var nearest = GetNearestTargetPoint(bot2.Pos, bot1.Pos, v => state.Matrix.Contains(v) && state.Bots.All(x => x.Pos != v));
						fussionQueue = fussionQueue ?? new Queue<ICommand>(GetMoveCommands(state, bot2, nearest, new HashSet<Vector>()).commands);

						var command = fussionQueue.Dequeue();

						var newList = GetFixCommand(state, command, bot2);

						if (newList.Count != 0)
						{
							fussionQueue = new Queue<ICommand>(newList.Concat(fussionQueue));
							ans.Add(fussionQueue.Dequeue());
						}
						else
						{
							ans.Add(command);
						}
						//ans.Add(command);
					}
				}
			}
			else if (state.Bots.Length != 8)
			{
				var commands = GetFirstCommads(state);
				foreach (var command in commands)
				{
					ans.Add(command);
				}
			}
			else
			{
				var task = buildingTasks.Peek();
				var taretPoints = task.Region.Points();

				commandsQueue = commandsQueue ?? GetCommadsQueue(state, taretPoints, task.Region);

				if (!commandsQueue.Any() || commandsQueue.All(x => x.Value.Count == 0))
				{
					commandsQueue = GetCommadsQueue(state, taretPoints, task.Region);
				}
				else
				{
					
				}

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
								
							var newList = GetFixCommand(state, moveCommand, bot);

							if (newList.Count != 0)
							{
								commandsQueue[bot.Bid] = new Queue<ICommand>(newList.Concat(commandsQueue[bot.Bid]));
								ans.Add(commandsQueue[bot.Bid].Dequeue());
							}
							else
							{
								ans.Add(moveCommand);
							}
						}
						else
						{
							ans.Add(new WaitCommand());
						}
					}
				}
			}

			if(ans.Count != state.Bots.Length)
				throw new InvalidOperationException();
			return ans;
		}


		private List<ICommand> GetFixCommand(State state, ICommand moveCommand, Bot bot)
		{
			if (moveCommand is SMoveCommand)
			{
				var command = (SMoveCommand) moveCommand;
				var d = command.Direction;

				var vector = bot.Pos + d;
				if(state.Matrix.IsVoid(vector))
					return new List<ICommand>();

				if (d.IsNd)
				{
					return new List<ICommand> {new VoidCommand(d), command};
				}

				var n_d = d.Normalize();
				var direction = d - n_d;
				return new List<ICommand> { new SMoveCommand(direction), new VoidCommand(n_d), new SMoveCommand(n_d) };
			}

			if (moveCommand is LMoveCommand)
			{
				var command = (LMoveCommand)moveCommand;
				var f = command.FirstDirection;
				var s = command.SecondDirection;

				var vector = bot.Pos + f + s;

				if (state.Matrix.IsVoid(vector))
					return new List<ICommand>();

				if (s.IsNd)
				{
					return new List<ICommand> { new SMoveCommand(f), new VoidCommand(s), new SMoveCommand(s) };
				}

				var n_s = s.Normalize();
				return new List<ICommand> { new LMoveCommand(f, s - n_s), new VoidCommand(n_s), new SMoveCommand(n_s) };
			}

			return new List<ICommand>();
		}

		private Dictionary<int, Queue<ICommand>> GetCommadsQueue(State state, List<Vector> points, Region region)
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
					//&& state.Matrix.IsVoid(v)
					&& !fobidden.Contains(v));

				if (point == null)
					continue;

				used.Add(point);
				
				fobidden.UnionWith(used);
				var (positions, moveCommands) = GetMoveCommands(state, bot, point, fobidden);
				if(positions.Any())
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
					var (positions, moveCommands) = GetMoveCommands(state, bot, point, fobidden);
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

		private (List<Vector> positions, List<ICommand> commands) GetMoveCommands(State state, Bot bot, Vector target, HashSet<Vector> fobidden)
		{
			if (bot.Pos == target)
				return (new List<Vector>(), new List<ICommand>());

			bool IsForbiddenArea(Vector v) => fobidden.Contains(v) || state.Bots.Any(x => x.Bid != bot.Bid && x.Pos == v);
			var botMoveSearcher = new BotMoveSearcher(state.Matrix, null, bot.Pos, IsForbiddenArea, state.Bots.Length, target, groundedChecker);

			var s = botMoveSearcher.FindPath(out var positions, out var commands, out _);

			if (!s)
			{
				return (null, null);
				//return (new List<Vector>(), new List<ICommand>());
			}

			return (positions, commands);
		}
	}
}