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
                var bots = state.Bots.OrderBy(b => b.Bid).ToList();
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
            //throw new NotImplementedException();
        }
    }
}
