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

        private void Validate(State state, Trace trace)
        {
            //throw new NotImplementedException();
        }
    }
}
