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
				.OrderByDescending(f => f.Mlen)
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
				.OrderByDescending(f => f.Mlen)
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

		public Dictionary<int, Vector> DoWork(Func<Vector, bool> isForbidden, out List<ICommand> commands, out List<Vector> volatiles)
		{
			if (figure.Where(isForbidden).Any())
				throw new ArgumentException("`figure` should not intersect `prohibited`");
			if (!figure.Contains(start))
				throw new ArgumentException("`figure` should contain `start`");

			var gravity = CalcGravity(figure, start);

			Console.WriteLine("figure.Count = {0}", figure.Count);
			Console.WriteLine("gravity.Count = {0}", gravity.Count);


			var voided = new HashSet<Vector>();
			voided.Add(start);
			var curPoint = start;

			volatiles = new List<Vector>();
			commands = new List<ICommand>();
			throw new NotImplementedException();
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

	}
}
