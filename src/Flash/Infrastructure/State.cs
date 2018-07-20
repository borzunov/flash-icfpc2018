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
        public long Energy { get; }
        public bool Harmonics { get; }
        public Bot[] Bots { get; }
    }
}
