using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Flash.Infrastructure.Models
{
    public class State
    {
        public State(long energy, bool harmonics, Matrix matrix, Bot[] bots, Trace trace, IOpLogWriter opLogWriter=null)
        {
            Energy = energy;
            Harmonics = harmonics;
            Matrix = matrix;
            Bots = bots;
            Trace = trace;
            OpLogWriter = opLogWriter ?? new FakeOpLog();
        }

        public static State CreateInitial(int r, IOpLogWriter opLogWriter=null)
        {
            return new State(0, false, new Matrix(r), new [] {new Bot(1, new Vector(0, 0, 0), Enumerable.Range(2, 39).ToArray())}, null,  opLogWriter);
        }

        public IOpLogWriter OpLogWriter { get; set; }
        public Trace Trace { get; }
        public Matrix Matrix { get; set; }
        public long Energy { get; set; }
        public bool Harmonics { get; set; }
        public Bot[] Bots { get; set; } // always sorted by id

        public bool IsValid()
        {
            return CheckGround() && CheckNanobotsIds() && CheckNanobotsPositions();
        }

        private bool CheckGround()
        {
            return Harmonics || Matrix.IsWellFormed();
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
