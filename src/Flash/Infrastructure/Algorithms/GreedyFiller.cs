using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class GreedyFiller : IWork
	{
		private readonly HashSet<Vector> figure;
		private readonly Func<Vector, bool> isForbidden;
		private readonly List<ICommand> commands;
		private readonly Matrix matrix;

		private int BotId;
		private Vector start;

		private readonly JsonOpLogWriter mongoOplogWriter;

		public GreedyFiller(Matrix matrix, HashSet<Vector> figure, JsonOpLogWriter mongoOplogWriter)
		{
			this.mongoOplogWriter = mongoOplogWriter;
			this.figure = figure;
			this.matrix = matrix;
		}

		public Vector GetPossibleStartPlace(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate)
		{
			return figure.OrderByDescending(f => f.Mlen)
				.Where(f => groundedChecker.CanPlace(f))
				.FirstOrDefault(f => f.GetAdjacents()
					.Any(adj => !figure.Contains(adj) && matrix.Contains(adj) && !isForbidden(botCoordinate)));
		}

		public Vector SetWorkerAndGetInput(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate, int botId)
		{
			if(start != null)
				throw new Exception("too many workers");

			BotId = botId;
			return start = figure.OrderBy(f => f.Mlen)
				.Where(f => groundedChecker.CanPlace(f))
				.FirstOrDefault(f =>f.GetAdjacents()
					.Any(adj => !figure.Contains(adj) && matrix.Contains(adj) && !isForbidden(botCoordinate)));
		}

		public bool IsEnoughWorkers()
		{
			return start != null;
		}

		public HashSet<Vector> GetWorkPlan()
		{
			return figure;
		}

		public Dictionary<int, Vector> DoWork(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, out List<ICommand> commands, out List<Vector> volatiles)
		{
			if (figure.Where(isForbidden).Any())
				throw new ArgumentException("`figure` should not intersect `prohibited`");
			if (!figure.Contains(start))
				throw new ArgumentException("`figure` should contain `start`");

			var gravity = CalcGravity(figure, start);
			
			Console.WriteLine("figure.Count = {0}", figure.Count);
			Console.WriteLine("gravity.Count = {0}", gravity.Count);
			
			var removeFills = new Dictionary<Vector, int>();
			var filled = new HashSet<Vector>();
			var curPoint = start;
			volatiles = new List<Vector>();
			commands = new List<ICommand>();
			while (filled.Count < figure.Count)
			{
				var nextPoint = curPoint.GetAdjacents()
					.Where(p => figure.Contains(p) && !filled.Contains(p))
					.OrderByDescending(p => gravity[p])
					.FirstOrDefault();
				if (nextPoint == null)
				{
					nextPoint = figure.Where(p => p != curPoint && !filled.Contains(p))
						.OrderBy(p => (curPoint - p).Mlen)
						.ThenByDescending(p => gravity[p])
						.FirstOrDefault();
					if (nextPoint != null)
					{
						mongoOplogWriter?.WriteColor(curPoint, "0000FF", 0.5);
						mongoOplogWriter?.WriteColor(nextPoint, "00FF00", 0.5);
					}

				}
				else
				{
					mongoOplogWriter?.WriteColor(nextPoint, "FF0000", 0.5);
				}
				if (nextPoint == null)
					break;

				try
				{
					List<Vector> curPath;
					List<ICommand> curCommands;
					if (!curPoint.IsAdjacentTo(nextPoint))
					{
						Move(figure, isForbidden, curPoint, nextPoint, true, out curPath, out curCommands);
					}
					else
					{
						MoveStraight(curPoint, nextPoint, out curPath, out curCommands);
					}
					foreach (var vector in curPath.Where(vector => filled.Contains(vector)))
					{
						removeFills[vector] = removeFills.TryGetValue(vector, out var val) ? val + 1 : 1;
						mongoOplogWriter?.WriteColor(vector, "FFFF00", 0.5);
					}
					
					filled.Add(curPoint);
					volatiles.AddRange(curPath);
					commands.AddRange(curCommands);
				}
				catch (ArgumentException)
				{
					Console.WriteLine("Exception, was able to draw only {0} points", filled.Count);
					break;
				}

				curPoint = nextPoint;
			}
			
			commands = commands.Where(command =>
			{
				if (removeFills.Count == 0)
					return true;
				var fillCommand = command as FillCommand;
				if (fillCommand == null)
					return true;

				if (!removeFills.TryGetValue(fillCommand.RealFill, out var val) || val == 0)
					return true;
				removeFills[fillCommand.RealFill]--;
				return false;
			}).ToList();
			
			return new Dictionary<int, Vector> {
				{ BotId, curPoint }
				};
		}

		private static void MoveStraight(Vector curPoint, Vector nextPoint, out List<Vector> curPath, out List<ICommand> curCommands)
		{
			curPath = new List<Vector>
			{
				nextPoint
			};
			curCommands = new List<ICommand>
			{
				new SMoveCommand(nextPoint - curPoint),
				new FillCommand(curPoint - nextPoint, curPoint),
			};
		}

		private Dictionary<Vector, int> CalcGravity(HashSet<Vector> figure, Vector end)
		{
			var dist = new Dictionary<Vector, int>();
			var order = new Queue<Vector>();
			dist[end] = 0;
			order.Enqueue(end);
			while (order.Count > 0)
			{
				var cur = order.Dequeue();
				foreach (var neigh in cur.GetAdjacents())
					if (figure.Contains(neigh) && !dist.ContainsKey(neigh))
					{
						var diff = neigh - cur;
						dist[neigh] = dist[cur] - diff.X - diff.Y * 4 - diff.Z * 2;
						order.Enqueue(neigh);
					}
			}
			return dist;
		}


		private bool Move(HashSet<Vector> figure, Func<Vector, bool> forbidden,
						  Vector start, Vector end, bool doFill,
						  out List<Vector> path, out List<ICommand> commands)
		{
			path = new List<Vector>();
			commands = new List<ICommand>();
			if (start.Equals(end))
				return true;

			var prev = new Dictionary<Vector, Tuple<Vector, ICommand>>();
			var order = new Queue<Vector>();
			prev[start] = null;
			order.Enqueue(start);
			var found = false;
			while (order.Count > 0)
			{
				var cur = order.Dequeue();

				var neighs = cur.GetAdjacents();
				foreach (var neigh in neighs)
				{
					if (!matrix.Contains(neigh) || !figure.Contains(neigh) || prev.ContainsKey(neigh) || forbidden(neigh))
						continue;

					prev[neigh] = Tuple.Create(cur, (ICommand)new SMoveCommand(neigh - cur));
					order.Enqueue(neigh);
					if (neigh == end)
					{
						found = true;
						break;
					}
				}
				if (found)
					break;
			}
			if (!found)
				return false;

			var point = end;
			while (point != start)
			{
				path.Add(point);
				var prevPoint = prev[point];

				if (doFill && figure.Contains(prevPoint.Item1))
				{
					commands.Add(new FillCommand(prevPoint.Item1 - point, prevPoint.Item1));
				}
				commands.Add(prevPoint.Item2);
				point = prevPoint.Item1;
			}
			path.Reverse();
			commands.Reverse();

			return true;
		}
	}
}
