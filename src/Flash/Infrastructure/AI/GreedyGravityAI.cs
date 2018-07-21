using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.AI
{
    public class GreedyGravityAI : IAI
    {
        private readonly Matrix targetMatrix;
        private readonly List<ICommand> commands;

        public GreedyGravityAI(Matrix targetMatrix)
        {
            this.targetMatrix = targetMatrix;
            var figure = new HashSet<Vector>();
			
	        for (var x = 0; x < targetMatrix.R; x++)
	        for (var y = 0; y < targetMatrix.R; y++)
	        for (var z = 0; z < targetMatrix.R; z++)
	        {
		        var point = new Vector(x, y, z);
		        if (targetMatrix.IsFull(point))
		        {
			        figure.Add(point);
		        }
	        }

			Console.WriteLine("Debug written");

            var start = figure.OrderBy(p => Tuple.Create(p.Z, p.Y, p.X)).First();
            var end = figure.OrderBy(p => Tuple.Create(p.Z, p.Y, p.X)).Last();
            Move(new HashSet<Vector> { }, new HashSet<Vector> { },
                new Vector(0, 0, 0), start, false, out var tmpPath, out commands);
            commands.AddRange(FillFigure(figure, new HashSet<Vector>(), start, end));
        }

        private List<ICommand> FillFigure(HashSet<Vector> figure, HashSet<Vector> prohibited,
                                Vector start, Vector end) {
            if (figure.Intersect(prohibited).Count() > 0)
                throw new ArgumentException("`figure` should not intersect `prohibited`");
            if (!(figure.Contains(start) && figure.Contains(end)))
                throw new ArgumentException("`figure` should contain `start` and `end`");

            var gravity = CalcGravity(figure, end);
	        var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
	        mongoOplogWriter.WriteLogName("GreedyGravityAI_Expected");
	        var state = State.CreateInitial(targetMatrix.R, mongoOplogWriter);
	        mongoOplogWriter.WriteInitialState(state);
			

			Console.WriteLine("figure.Count = {0}", figure.Count);
            Console.WriteLine("gravity.Count = {0}", gravity.Count);
            figure = gravity.Keys.ToHashSet(); // FIXME: It's a hack for non-connected figures

	        int i = 0;

            var filled = new HashSet<Vector>();
            var curPoint = start;
            var volatiles = new List<Vector>();
            var commands = new List<ICommand>();
            while (filled.Count < figure.Count)
            {
	            var time = DateTime.UtcNow;
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
					if(nextPoint != null)
						mongoOplogWriter.WriteColor(nextPoint, "00FF00", 0.5);
				}
                else
                {
					mongoOplogWriter.WriteColor(nextPoint, "FF0000", 0.5);
				}
                if (nextPoint == null)
                    nextPoint = new Vector(0, 0, 0);
				
                try
                {
	                List<Vector> curPath;
	                List<ICommand> curCommands;
					if (!curPoint.IsAdjacentTo(nextPoint))
						Move(filled, prohibited, curPoint, nextPoint, true, out curPath, out curCommands);
					else
					{
						curPath = new List<Vector>
						{
							nextPoint
						};
						curCommands = new List<ICommand>
							{
								new SMoveCommand(nextPoint - curPoint),
								new FillCommand(curPoint - nextPoint),
							};
					}
                    filled.Add(curPoint);
                    volatiles.AddRange(curPath);
                    commands.AddRange(curCommands);
                } catch (ArgumentException)
                {
                    Console.WriteLine("Exception, was able to draw only {0} points", filled.Count);
                    break;
                }

				curPoint = nextPoint;
			}
			
	        mongoOplogWriter.Save();

			commands.Add(new HaltCommand());
            Console.WriteLine("Commands have been generated");

            return commands;
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
                        dist[neigh] = dist[cur] + 1;
                        order.Enqueue(neigh);
                    }
            }
            return dist;
        }

        private void Move(HashSet<Vector> filled, HashSet<Vector> prohibited,
                          Vector start, Vector end, bool doFill,
                          out List<Vector> path, out List<ICommand> commands)
        {
            // TODO: LMove, Dijkstra

            path = new List<Vector>();
            commands = new List<ICommand>();
            if (start.Equals(end))
                return;

            var prev = new Dictionary<Vector, Tuple<Vector, ICommand>>();
            var order = new Queue<Vector>();
            prev[start] = null;
            order.Enqueue(start);
            var found = false;
            while (order.Count > 0)
            {
                var cur = order.Dequeue();
                var neighs = (doFill && cur == start
                    ? (IEnumerable<Vector>) cur.GetAdjacents()
                    : cur.GetLongLinearNeighs());
                foreach (var neigh in neighs)
                {
                    if (!targetMatrix.Contains(neigh) || prev.ContainsKey(neigh) ||
                        GetPointsTowards(cur, neigh).Any(p => filled.Contains(p) || prohibited.Contains(p)))
                        continue;

                    prev[neigh] = Tuple.Create(cur, (ICommand) new SMoveCommand(neigh - cur));
                    order.Enqueue(neigh);
                    if (neigh == end)
                    {
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
                throw new ArgumentException("`end` is unreachable from `start`");

            var point = end;
            while (point != start)
            {
                path.Add(point);
                commands.Add(prev[point].Item2);
                point = prev[point].Item1;
            }
            path.Reverse();
            commands.Reverse();

            if (doFill)
                commands.Insert(1, new FillCommand(start - path[0]));
        }

        private IEnumerable<Vector> GetPointsTowards(Vector start, Vector end)
        {
            var direction = end - start;
            direction = new Vector(Clip(direction.X, 1), Clip(direction.Y, 1), Clip(direction.Z, 1));

            var point = start;
            while (!point.Equals(end))
            {
                point += direction;
                yield return point;
            }
        }

        private int Clip(int value, int abs)
        {
            if (value < -abs)
                return -abs;
            if (value > abs)
                return abs;
            return value;
        }

        private int curCommand = 0;

        public IEnumerable<ICommand> NextStep(State state)
        {
            return new List<ICommand> { commands[curCommand++] };
        }
    }
}
