using System;
using System.Linq;

namespace Flash.Infrastructure.Simulation
{
    public class Simulator
    {
        public State NextStep(State state, Trace trace)
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

            return state;
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
