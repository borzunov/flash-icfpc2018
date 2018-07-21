using System.Linq;

namespace Flash.Infrastructure
{
    public class State
    {
        public State(long energy, bool harmonics, Matrix matrix, Bot[] bots, Trace trace)
        {
            Energy = energy;
            Harmonics = harmonics;
            Matrix = matrix;
            Bots = bots;
            Trace = trace;
        }

        public Trace Trace { get; }
        public Matrix Matrix { get; }
        public long Energy { get; set; }
        public bool Harmonics { get; set; }
        public Bot[] Bots { get; set; } // always sorted by id

        public bool IsValid()
        {
            return CheckGround() && CheckNanobotsIds() && CheckNanobotsPositions();
        }

        private bool CheckGround()
        {
            return Harmonics || Matrix.IsGrounded(); //TODO optimize IsGrounded
        }

        private bool CheckNanobotsPositions()
        {
            return Bots.Select(b => b.Pos).Distinct().Count() == Bots.Length && Bots.Select(b => b.Pos).All(Matrix.IsVoid);
        }

        private bool CheckNanobotsIds()
        {
            var activeBids = Bots.Select(b => b.Bid);
            var seeds = Bots.SelectMany(b => b.Seeds);
            var allBids = activeBids.Concat(seeds).ToList();
            return allBids.Distinct().Count() == allBids.Count;
        }
    }
}
