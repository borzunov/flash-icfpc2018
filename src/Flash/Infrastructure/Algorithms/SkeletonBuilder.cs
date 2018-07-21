using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class SkeletonBuilder
	{
		private const int Void = -1;
		private const int Skeleton = -2;

		private int R;
		private int BotsCount;
		private Dictionary<Vector, int> VectorComponents;
		private Dictionary<Vector, bool> IsMoveableMatrix = new Dictionary<Vector, bool>();

		public SkeletonBuilder(int r, List<Vector>[] components)
		{
			VectorComponents = new Dictionary<Vector, int>();
			for (var index = 0; index < components.Length; index++)
			{
				var component = components[index];
				foreach (var vector in component)
				{
					VectorComponents[vector] = index;
				}
			}

			BotsCount = components.Length;
			R = r;
		}

		private void SetMovable()
		{
			foreach (var vectorPair in VectorComponents)
			{
				var vector = vectorPair.Key;
				if (VectorComponents.TryGetValue(vector, out var state) ? state == Skeleton : false)
				{
					IsMoveableMatrix[vector] = false;
					continue;
				}

				IsMoveableMatrix[vector] = new[] { vector }.Concat(vector.GetAdjacents())
					.Select(vec => VectorComponents.TryGetValue(vec, out var val) ? val : Void)
					.Where(s => s != Skeleton)
					.Distinct().Any();
			}
		}

		private bool IsMoveable(Vector vector)
		{
			return IsMoveableMatrix.TryGetValue(vector, out var moveable) ? moveable : Matrix.Contains(R, vector);
		}

		public SkeletonNode FindSkeleton()
		{
			var skeleton = new SkeletonNode();
			
			HashSet<int> unvisitedAreas = Enumerable.Range(0, BotsCount).ToHashSet();
			Dictionary<Vector, Vector> dads = new Dictionary<Vector, Vector>();
			Dictionary<Vector, SkeletonNode> skeletons = new Dictionary<Vector, SkeletonNode>();

			var queue = new Queue<Vector>();
			queue.Enqueue(new Vector(0, 0, 0));
			dads[new Vector(0, 0, 0)] = null;

			while (unvisitedAreas.Count > 0)
			{
				var node = queue.Dequeue();

				if (VectorComponents.TryGetValue(node, out var component) && unvisitedAreas.Contains(component))
				{
					unvisitedAreas.Remove(component);
					
					//var branch
				}


				foreach (var adj in node.GetAdjacents().Where(adj => !dads.ContainsKey(adj) && IsMoveable(adj)))
				{
					dads[adj] = node;
				}
			}
			throw new Exception();
		}
	}

	class SkeletonNode
	{
		public Vector Vector;
		public Dictionary<int, SkeletonNode> Childs = new Dictionary<int, SkeletonNode>();
	}
}
