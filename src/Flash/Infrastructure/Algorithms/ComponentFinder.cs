using System.Collections.Generic;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
    public class ComponentFinder
    {
        private readonly int R;
        private readonly Matrix xorMatrix, targetMatrix;
        private readonly int[,,] pointComp;
        private List<Component> components;
        
        public ComponentFinder(Matrix xorMatrix, Matrix targetMatrix)
        {
            R = xorMatrix.R;
            this.xorMatrix = xorMatrix;
            this.targetMatrix = targetMatrix;

            pointComp = new int[R, R, R];
            for (var x = 0; x < R; x++)
            for (var y = 0; y < R; y++)
            for (var z = 0; z < R; z++)
                pointComp[x, y, z] = -1;
        }

        public List<Component> Find()
        {
            components = new List<Component>();
            for (var x = 0; x < R; x++)
            for (var y = 0; y < R; y++)
            for (var z = 0; z < R; z++)
            {
                var point = new Vector(x, y, z);
                if (xorMatrix.IsVoid(point) || pointComp[x, y, z] != -1)
                    continue;
                
                components.Add(TraverseComponent(components.Count, point));
            }
            return components;
        }

        private Component TraverseComponent(int compIndex, Vector point)
        {
            pointComp[point.X, point.Y, point.Z] = compIndex;
            var points = new List<Vector> { point };
            var neighs = new HashSet<int>();
            for (var i = 0; i < points.Count; i++)
            {
                var cur = points[i];
                foreach (var neigh in cur.GetAdjacents())
                    if (xorMatrix.Contains(neigh) && xorMatrix.IsFull(cur))
                    {
                        var neighComp = pointComp[neigh.X, neigh.Y, neigh.Z];
                        if (targetMatrix.IsFull(cur) == targetMatrix.IsFull(neigh) &&
                            neighComp == -1)
                        {
                            pointComp[neigh.X, neigh.Y, neigh.Z] = compIndex;
                            points.Add(neigh);
                        }
                        else if (targetMatrix.IsFull(cur) != targetMatrix.IsFull(neigh) &&
                            neighComp != -1)
                        {
                            components[neighComp].Neighs.Add(compIndex);
                            neighs.Add(neighComp);
                        }
                    }
            }
            return new Component(points, neighs);
        }
    }
}
