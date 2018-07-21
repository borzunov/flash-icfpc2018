using System;
using System.Linq;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Simulation
{
    public class Simulator
    {
        public void NextStep(State state, Trace trace)
        {
            if (state.Bots.Length != trace.Count)
                throw new Exception("Commands count should be equal to bots count");

            Validate(state, trace);

            UpdateEnergy(state);

            while (trace.Any())
            {
                var bots = state.Bots.OrderBy(b => b.Bid).ToList();
                var commands = trace.Dequeue(bots.Count);

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

        private void Validate(State state, Trace trace)
        {
            //throw new NotImplementedException();
        }
    }
}
