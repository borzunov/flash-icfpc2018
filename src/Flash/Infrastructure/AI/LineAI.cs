using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure
{
    public class LineAI : IAI
    {
        private readonly Matrix modelToDraw;

        public LineAI(Matrix modelToDraw)
        {
            this.modelToDraw = modelToDraw;
        }

        public IEnumerable<ICommand> NextStep(State state)
        {

            var bot = state.Bots[0];
            (Vector toGo, Vector toDraw) = Bfs(bot, state);


            if (state.Bots.Length > 1 && toGo.Mlen != 0)
                throw new InvalidOperationException();

            if (toGo == null)
            {
                yield return new HaltCommand();
                yield break;
            }
            if (state.Bots.Length == 1 && toGo.Mlen != 0)
            {
                yield return new SMoveCommand(toGo);
            }
            else if (toGo.Mlen == 0) // пришли рисовать
            {
                var max = int.MinValue;
                Vector destination = null;
                Vector direction = null; //unit

                for (var i = 0; i < 6; i++)
                {
                    var curMax = 0;

                    for (var n = 1; n < 30; n++)
                    {
                        var adj = toDraw.GetAdjacents(n)[i];
                        if (!adj.IsGood(state.Matrix.R) || state.Matrix.IsFull(adj) || bot.Pos.Equals(adj) ||
                            modelToDraw.IsVoid(adj)
                            || state.Bots.Length > 1 && state.Bots[1].Pos.Equals(adj))
                        {
                            break;
                        }

                        curMax++;
                        if (curMax > max)
                        {
                            max = curMax;
                            destination = adj;
                            direction = toDraw.GetAdjacents(1)[i];
                        }
                    }
                }

                if (destination == null)
                {
                    yield return new FillCommand(toDraw - state.Bots[0].Pos);
                    if (state.Bots.Length > 1)
                        yield return new WaitCommand();
                }
                else if (state.Bots.Length == 1)
                {
                    var placeToSpawn = bot.Pos.GetNears().Intersect(toDraw.GetNears())
                        .FirstOrDefault(v => state.Matrix.IsVoid(v) && !v.Equals(bot.Pos + direction));
                    if (placeToSpawn == null)
                    {
                        yield return new FillCommand(toDraw - state.Bots[0].Pos);
                        //yield return JumpToRandomPlace();
                    }
                    else
                        yield return new FissionCommand(placeToSpawn - bot.Pos, 1);
                }
                else
                {
                    var nearDistance1 = toDraw - state.Bots[0].Pos;
                    yield return new GFillCommand(nearDistance1, destination - toDraw);
                    var nearDistance2 = toDraw - state.Bots[1].Pos;
                    yield return new GFillCommand(nearDistance2, destination - toDraw);
                    yield return new FusionPCommand(state.Bots[1].Pos - state.Bots[0].Pos);
                    yield return new FusionSCommand(state.Bots[0].Pos - state.Bots[1].Pos);
                }
            }
        }

        private ICommand JumpToRandomPlace()
        {
            throw new NotImplementedException();
        }

        public (Vector toGo, Vector toDraw) Bfs(Bot bot, State state)
        {
            var parents = new Dictionary<Vector, Vector>();
            var visited = new HashSet<Vector>();
            var queue = new Queue<Vector>();
            queue.Enqueue(bot.Pos);

            var toDraw1 = bot.Pos.GetNears().FirstOrDefault(x => x.IsGood(state.Matrix.R) && modelToDraw.IsFull(x) && state.Matrix.IsVoid(x));
            if (toDraw1 != null)
            {
                return (new Vector(0,0,0), toDraw1);
            }

            while (queue.Any())
            {
                var cur = queue.Dequeue();
                for (var i = 0; i < 6; i++)
                {
                    for (var n = 1; n < 15; n++)
                    {
                        var adj = cur.GetAdjacents(n)[i];
                        if (!adj.IsGood(state.Matrix.R) || state.Matrix.IsFull(adj))
                            break;

                        if (!visited.Contains(adj))
                        {
                            parents.Add(adj, cur);
                            queue.Enqueue(adj);
                            visited.Add(adj);

                            var toDraw = adj.GetNears().FirstOrDefault(x => x.IsGood(state.Matrix.R) && modelToDraw.IsFull(x) && state.Matrix.IsVoid(x));
                            if(toDraw != null)
                            { 
                                var curr = adj;
                                while (!Equals(parents[curr], bot.Pos))
                                {
                                    curr = parents[curr];
                                }

                                return (curr - bot.Pos, toDraw);
                            }
                        }
                    }
                }
            }

            return (null, null);
        }

    }
}