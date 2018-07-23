using System.Collections.Generic;

namespace Flash.Infrastructure.Models
{
    public class Component
    {
        public readonly List<Vector> Points;
        public readonly HashSet<int> Neighs;

        public Component(List<Vector> points, HashSet<int> neighs)
        {
            Points = points;
            Neighs = neighs;
        }
    }
}
