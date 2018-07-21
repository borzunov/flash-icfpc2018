using System.Collections.Generic;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Alghorithms
{
    public class BridgesFinder
    {
        private readonly bool[,,] matrix;
        public int R => matrix.GetLength(0);

        public HashSet<Vector> Bridges { get; private set; }
        public HashSet<HashSet<Vector>> ConnectedComponents { get; private set; }

        private BridgesFinder(int r)
        {
            matrix = new bool[r, r, r];

            Bridges = new HashSet<Vector>();
            ConnectedComponents = new HashSet<HashSet<Vector>>();
        }

        private BridgesFinder(bool[,,] matrix)
        {
            this.matrix = matrix;

            Bridges = new HashSet<Vector>();
            ConnectedComponents = new HashSet<HashSet<Vector>>();

            FullRecomputing();
        }


        public static BridgesFinder Create(int r)
        {
            return new BridgesFinder(r);
        }

        public static BridgesFinder Create(bool[,,] matrix)
        {
            return new BridgesFinder(matrix);
        }

        private void FullRecomputing()
        {
            
        }
    }
}
