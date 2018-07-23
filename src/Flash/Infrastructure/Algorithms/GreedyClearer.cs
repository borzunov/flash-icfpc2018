using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class GreedyClearer : IWork
	{
		private readonly HashSet<Vector> figure;
		private readonly Func<Vector, bool> isForbidden;
		private readonly List<ICommand> commands;
		private readonly Matrix matrix;

		private int BotId;
		private Vector start;

		private readonly JsonOpLogWriter mongoOplogWriter;
		
		public GreedyClearer(Matrix matrix, HashSet<Vector> figure, JsonOpLogWriter mongoOplogWriter)
		{
			this.mongoOplogWriter = mongoOplogWriter;
			this.figure = figure;
			this.matrix = matrix;
		}

		public Vector GetPossibleStartPlace(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate)
		{
			return figure
				.OrderByDescending(f => f.GetAdjacents().Count(f1 => matrix.Contains(f1) && matrix.IsFull(f1)) == 1)
				.ThenByDescending(f => f.Mlen)
				.Where(groundedChecker.CanRemove)
				.FirstOrDefault(f => f.GetAdjacents()
					.Any(adj => !figure.Contains(adj) && matrix.Contains(adj) && !isForbidden(botCoordinate)));
		}

		public Vector SetWorkerAndGetInput(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate, int botId)
		{
			if (start != null)
				throw new Exception("too many workers");

			BotId = botId;
			return start = figure
				.OrderByDescending(f => f.GetAdjacents().Count(f1 => matrix.Contains(f1) && matrix.IsFull(f1)) == 1)
				.ThenByDescending(f => f.Mlen)
				.Where(groundedChecker.CanRemove)
				.FirstOrDefault(f => f.GetAdjacents()
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


			var voided = new HashSet<Vector> {start};
			if(matrix.IsFull(start))
				groundedChecker.UpdateWithClear(start);
			var curPoint = start;

			foreach (var vector in figure.Where(f => matrix.IsVoid(f)))
			{
				voided.Add(vector);
			}

			volatiles = new List<Vector>();
			commands = new List<ICommand>();


			while (voided.Count < figure.Count)
			{
				var nextPoint = curPoint.GetAdjacents()
					.Where(p => figure.Contains(p) && !voided.Contains(p))
					.Where(groundedChecker.CanRemove)
					.OrderByDescending(f => f.GetAdjacents().Count(f1 => matrix.Contains(f1) && matrix.IsFull(f1) && !voided.Contains(f1)) == 1)
					.ThenByDescending(p => gravity[p])
					.FirstOrDefault();
				
				List<Vector> curPath;
				List<ICommand> curCommands;
				if (nextPoint == null)
				{
					nextPoint = Move(groundedChecker, figure, voided, isForbidden, curPoint, out curPath, out curCommands);
				}
				else
				{
					MoveStraight(curPoint, nextPoint, out curPath, out curCommands);
				}

				if (nextPoint == null)
				{
					var rest = figure.Where(f => !voided.Contains(f)).ToList();
					break;
				}

				foreach (var vector in curPath.Where(vector => voided.Contains(vector)))
				{
					mongoOplogWriter?.WriteColor(vector, "FFFF00", 0.5);
				}

				foreach (var deletedPoint in curCommands.OfType<VoidCommand>().Select(c => c.RealVoid))
				{
					if(!voided.Add(deletedPoint))
						throw new Exception("already removed");
					groundedChecker.UpdateWithClear(deletedPoint);
				}
				commands.AddRange(curCommands);
				volatiles.AddRange(curPath);
				
				curPoint = nextPoint;
			}

			foreach (var vector in commands.OfType<VoidCommand>().Select(c => c.RealVoid).Reverse())
			{
				groundedChecker.UpdateWithFill(vector);
			}

			return new Dictionary<int, Vector>{{ BotId, curPoint } };
		}

		private static void MoveStraight(Vector curPoint, Vector nextPoint, out List<Vector> curPath, out List<ICommand> curCommands)
		{
			curPath = new List<Vector>
			{
				nextPoint
			};
			curCommands = new List<ICommand>
			{
				new VoidCommand(nextPoint - curPoint, nextPoint),
				new SMoveCommand(nextPoint - curPoint),
			};
		}

		private Vector Move(IsGroundedChecker groundedChecker, HashSet<Vector> figure, HashSet<Vector> voided, Func<Vector, bool> forbidden, Vector start,  
			out List<Vector> path, out List<ICommand> commands)
		{
			path = new List<Vector>();
			commands = new List<ICommand>();

			var prev = new Dictionary<Vector, Tuple<Vector, List<ICommand>>>();
			var order = new Queue<Vector>();
			prev[start] = null;
			order.Enqueue(start);
			var found = false;
			Vector end = null;

			while (order.Count > 0)
			{
				var cur = order.Dequeue();

				var neighs = cur.GetAdjacents();
				foreach (var neigh in neighs)
				{
					if (!voided.Contains(neigh) && figure.Contains(neigh))
					{
						if (neigh == new Vector(15, 4, 13))
						{
							var diff = figure.Where(f => !voided.Contains(f)).ToList();
							Console.WriteLine();
						}

						if (groundedChecker.CanRemove(neigh))
						{
							found = true;
							end = neigh;
							prev[neigh] = Tuple.Create(cur, new List<ICommand> { new SMoveCommand(neigh - cur), new VoidCommand(neigh - cur, neigh) });
							break;
						}
						continue;
					}

					if (!matrix.Contains(neigh) || (figure.Contains(neigh) ? !voided.Contains(neigh) : matrix.IsFull(neigh)) || prev.ContainsKey(neigh) || forbidden(neigh))
						continue;

					prev[neigh] = Tuple.Create(cur, new List<ICommand> { new SMoveCommand(neigh - cur) });
					order.Enqueue(neigh);
				}
				if (found)
					break;
			}
			if (!found)
				return null;

			var point = end;
			while (point != start)
			{
				path.Add(point);
				var prevPoint = prev[point];
				
				commands.AddRange(prevPoint.Item2);
				point = prevPoint.Item1;
			}
			path.Reverse();
			commands.Reverse();

			return end;
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
						dist[neigh] = dist[cur] - diff.X + diff.Y * 4 - diff.Z * 2;
						order.Enqueue(neigh);
					}
			}
			return dist;
		}

	}
}
