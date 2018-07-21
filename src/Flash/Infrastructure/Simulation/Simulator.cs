using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Simulation
{
    public class Simulator
    {
        public void NextStep(State state, Trace trace)
        {
            if (state.Bots.Length != trace.Count)
                throw new Exception("Commands count should be equal to bots count");

           

            UpdateEnergy(state);

            while (trace.Any())
            {
                var bots = state.Bots.ToList();
                var commands = trace.Dequeue(bots.Count);
                Validate(state, bots, commands);


                for (var i = 0; i < bots.Count; i++)
                {
                    var command = commands[i];

                    command.Apply(state, bots[i]);
                }
            }
        }

        private static void UpdateEnergy(State state)
        {
            var r = state.Matrix.R;
            if (state.Harmonics)
            {
                state.Energy += 30*r*r*r;
            }
            else
            {
                state.Energy += 3*r*r*r;
            }

            state.Energy += 20*state.Bots.Length;
        }

        private void Validate(State state, List<Bot> bots, ICommand[] commands)
        {
            var regions = new List<Region>();
            for (var i = 0; i < bots.Count; i++)
            {
                switch (commands[i])
                {
                    case FillCommand f:
                        regions.Add(new Region(bots[i].Pos));
                        regions.Add(new Region(bots[i].Pos + f.NearDistance));
                        break;
                    case FissionCommand f:
                        regions.Add(new Region(bots[i].Pos));
                        regions.Add(new Region(bots[i].Pos + f.NearDistance));
                        break;
                    case FlipCommand f:
                    case HaltCommand h:
                    case WaitCommand w:
                        regions.Add(new Region(bots[i].Pos));
                        break;
                    case LMoveCommand l:
                        regions.Add(new Region(bots[i].Pos, bots[i].Pos + l.FirstDirection));
                        regions.Add(new Region((bots[i].Pos + l.FirstDirection) + l.SecondDirection.Normalize(), bots[i].Pos + l.FirstDirection + l.SecondDirection));
                        break;
                    case SMoveCommand s:
                        regions.Add(new Region(bots[i].Pos, bots[i].Pos + s.Direction));
                        break;
                    case FusionPCommand f:
                        regions.Add(new Region(bots[i].Pos));
                        regions.Add(new Region(bots[i].Pos + f.NearDistance));
                        break;
                }
            }

            for (var i = 0; i < regions.Count-1; i++)
            {
                for (var j = i+1; j < regions.Count; j++)
                {
                    if (Intersects(regions[i], regions[j]))
                    {
                        throw new Exception("Commands used regionst itersects");
                    }
                }
            }
        }

        private static bool Intersects(Region r1, Region r2)
        {
            int x1, y1, x2, y2, x3, y3, x4, y4;
            if (r1.Min.X == r1.Max.X && r1.Max.X == r2.Min.X && r2.Min.X == r2.Max.X)
            {
                x1 = r1.Min.Y;
                y1 = r1.Min.Z;

                x2 = r1.Max.Y;
                y2 = r1.Max.Z;

                x3 = r2.Min.Y;
                y3 = r2.Min.Z;

                x4 = r2.Max.Y;
                y4 = r2.Max.Z;
            } 
            else if (r1.Min.Y == r1.Max.Y && r1.Max.Y == r2.Min.Y && r2.Min.Y == r2.Max.Y)
            {
                x1 = r1.Min.X;
                y1 = r1.Min.Z;
                
                x2 = r1.Max.X;
                y2 = r1.Max.Z;
                
                x3 = r2.Min.X;
                y3 = r2.Min.Z;
                
                x4 = r2.Max.X;
                y4 = r2.Max.Z;
            }
            else if (r1.Min.Z == r1.Max.Z && r1.Max.Z == r2.Min.Z && r2.Min.Z == r2.Max.Z)
            {
                x1 = r1.Min.X;
                y1 = r1.Min.Y;

                x2 = r1.Max.X;
                y2 = r1.Max.Y;

                x3 = r2.Min.X;
                y3 = r2.Min.Y;

                x4 = r2.Max.X;
                y4 = r2.Max.Y;
            }
            else
            {
                return false;
            }

            return x1 <= x3 && x3 <= x2 && x1 <= x4 && x4 <= x2 &&
                   y3 <= y2 && y2 <= y4 && y3 <= y1 && y1 <= y4;
        }
    }
}
