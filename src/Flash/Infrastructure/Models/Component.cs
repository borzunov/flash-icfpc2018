using System.Collections.Generic;

namespace Flash.Infrastructure.Models
{
    public class Component
    {
        public readonly bool Fill;
        public readonly List<Vector> Points;
        public readonly HashSet<int> Neighs;

        public Component(bool fill, List<Vector> points, HashSet<int> neighs)
        {
            Fill = fill;
            Points = points;
            Neighs = neighs;
        }
    }
}
