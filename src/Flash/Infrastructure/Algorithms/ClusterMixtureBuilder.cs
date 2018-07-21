using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class ClusterMixtureBuilder
	{
		private int R;

		public int BotsCount;
		public double[,,][] Weights;
		public double[,,][] NormalizedWeights;
		public double[,,][] Distances;
		public Cluster[] Clusters;
		public int[] ClustersSizes;

		public HashSet<Vector> Voxels;

		public List<double[]> VoxelsWeights => Voxels.Select(voxel => Weights[voxel.X, voxel.Y, voxel.Z]).ToList();
		public List<double[]> VoxelsNormalizedWeights => Voxels.Select(voxel => NormalizedWeights[voxel.X, voxel.Y, voxel.Z]).ToList();

		public ClusterMixtureBuilder(Matrix matrix, int botsCount)
		{
			R = matrix.R;

			BotsCount = botsCount;
			Clusters = new Cluster[BotsCount];
			Weights = new double[R, R, R][];
			NormalizedWeights = new double[R, R, R][];
			Distances = new double[R, R, R][];

			var voxels = new List<Vector>();

			for (int x = 0; x < R; x++)
			{
				for (int y = 0; y < R; y++)
				{
					for (int z = 0; z < R; z++)
					{
						if (matrix.IsFull(new Vector(x, y, z)))
						{
							Weights[x, y, z] = Clusters.Select(c => 0.0).ToArray();
							NormalizedWeights[x, y, z] = Clusters.Select(c => 0.0).ToArray();
							Distances[x,y,z] = Clusters.Select(c => 0.0).ToArray();
							
							voxels.Add(new Vector(x, y, z));
						}
					}
				}

				Voxels = voxels.ToHashSet();
			}

			var rand = new Random();

			for (var index = 0; index < Clusters.Length; index++)
			{
				var cluster = Clusters[index] = new Cluster();
				var newCenter = voxels[rand.Next(voxels.Count)];
				while (Clusters.Any(cluster1 => cluster1?.Center != null && cluster1.Center.Equals(newCenter)))
					newCenter = voxels[rand.Next(voxels.Count)];
				cluster.Center = newCenter;
			}

			ClustersSizes = Clusters.Select(cluster => voxels.Count / Clusters.Length).ToArray();
		}

		public TreeNode[] ClusterRoots;

		public void CountWeights(int penalty)
		{
			Random rand = new Random();
			ClusterRoots = new TreeNode[Clusters.Length];

			for (int i = 0; i < Clusters.Length; i++)
			{
				var distCount = new Dictionary<int, int>();
				var clusterSize = ClustersSizes[i];
				var count = 0;

				ClusterRoots[i] = new TreeNode(Clusters[i].Center);

				var usedPoints = new HashSet<Vector>();
				var queue = new Queue<Tuple<Vector, int, TreeNode>>();
				queue.Enqueue(Tuple.Create(Clusters[i].Center, 0, ClusterRoots[i]));
				usedPoints.Add(Clusters[i].Center);

				while (queue.Count > 0)
				{
					var node = queue.Dequeue();
					count++;
					distCount[node.Item2] = count;
					Weights[node.Item1.X, node.Item1.Y, node.Item1.Z][i] = Math.Max(clusterSize - count, 1.0 / (node.Item2+1));

					foreach (var near in node.Item1.GetAdjacents()
						.OrderBy(_ => rand.NextDouble())
						.Where(near => !usedPoints.Contains(near) && Voxels.Contains(near)))
					{

						queue.Enqueue(Tuple.Create(near, node.Item2 + 1, node.Item3.AddChild(near)));
						usedPoints.Add(near);
					}
				}
			}
		}

		public void NormalizeWeights()
		{
			for (int x = 0; x < R; x++)
			{
				for (int y = 0; y < R; y++)
				{
					for (int z = 0; z < R; z++)
					{
						if(Weights[x,y,z] == null)
							continue;
						var sum = Weights[x, y, z].Sum();

						for (int i = 0; i < Weights[x,y,z].Length; i++)
						{
							NormalizedWeights[x, y, z][i] = Weights[x, y, z][i] / sum;
						}
					}
				}
			}
		}

		public double CountMetric()
		{
			double sum = 0;

			for (int x = 0; x < R; x++)
			{
				for (int y = 0; y < R; y++)
				{
					for (int z = 0; z < R; z++)
					{
						if (Weights[x, y, z] == null)
							continue;

						for (int i = 0; i < Weights[x, y, z].Length; i++)
						{
							sum += Weights[x, y, z][i] * NormalizedWeights[x, y, z][i];
						}
					}
				}
			}

			return sum;
		}

		public void UpdateCenters()
		{
			var components = GetComponents();
			for (var index = 0; index < ClusterRoots.Length; index++)
			{
				var sum = 0.0;
				var sumX = 0.0;
				var sumY = 0.0;
				var sumZ = 0.0;

				foreach (var voxel in components[index])
				{
					var weight = 1.0;// Weights[voxel.X, voxel.Y, voxel.Z][index];
					sum += weight;
					sumX += weight * voxel.X;
					sumY += weight * voxel.Y;
					sumZ += weight * voxel.Z;
				}

				sumX /= sum;
				sumY /= sum;
				sumZ /= sum;

				var minDist = double.MaxValue;
				Vector center = null;
				foreach (var voxel in Voxels)
				{
					var dist = Math.Abs(voxel.X - sumX) + Math.Abs(voxel.Y - sumY) + Math.Abs(voxel.Z - sumZ);
					if (dist < minDist)
					{
						minDist = dist;
						center = voxel;
					}
				}
			
				Clusters[index].Center = center;
			}
		}

		public List<Vector>[] GetComponents()
		{
			var result = new List<Vector>[Clusters.Length];

			for (int i = 0; i < Clusters.Length; i++)
			{
				result[i] = new List<Vector>();
			}

			foreach (var node in Voxels)
			{
				var max = -1.0;
				var argmax = 0;

				for (int i = 0; i < Clusters.Length; i++)
				{
					if (NormalizedWeights[node.X, node.Y, node.Z][i] > max)
					{
						max = NormalizedWeights[node.X, node.Y, node.Z][i];
						argmax = i;
					}
				}

				result[argmax].Add(node);
			}

			return result;
		}

		public List<Vector>[] BuildClusterMixture()
		{
			List<Vector>[] maxMixture = null;
			var mixMax = int.MaxValue;

			for (int i = 0; i < 100; i++)
			{
				CountWeights(i/5+1);
				NormalizeWeights();
				Console.WriteLine(CountMetric());
				var res = GetComponents();
				for (int j = 0; j < Clusters.Length; j++)
				{
					Console.WriteLine($"coponent {j}: {res[j].Count} nodes");
				}

				var max = res.Max(c => c.Count);
				if (max < mixMax)
				{
					maxMixture = res;
					mixMax = max;
				}

				UpdateCenters();
			}

			return maxMixture;
		}
	}

	class TreeNode
	{
		public TreeNode Dad;
		public Vector Node;
		public List<TreeNode> Children = new List<TreeNode>();
		public double Force;
		public double AggregatedForce;
		public double SummaryForce;

		public TreeNode(Vector node)
		{
			Dad = null;
			Node = node;
		}

		public TreeNode AddChild(Vector vec)
		{
			var node = new TreeNode(vec) {Dad = this};
			Children.Add(node);
			return node;
		}

		public double UpdateForces(double[,,][] weights, int cluster)
		{
			Force = weights[Node.X, Node.Y, Node.Z][cluster];
			AggregatedForce = Force + Children.Sum(c => c.UpdateForces(weights, cluster));
			return AggregatedForce;
		}

		public void UpdateSummaryForces(double dadForce)
		{
			var maxForce = dadForce;
			var minForce = AggregatedForce;
			foreach (var child in Children)
			{
				child.UpdateSummaryForces(Children.Where(c => c != child).Select(c => c.AggregatedForce).Concat(new []{dadForce}).Sum());
				maxForce = Math.Max(child.AggregatedForce, maxForce);
				minForce = Math.Min(child.AggregatedForce, minForce);
			}

			SummaryForce = maxForce - minForce;
		}

		public TreeNode GetMinimumForceNode()
		{
			var minForceNode = this;
			foreach (var child in Children)
			{
				var cand = child.GetMinimumForceNode();
				if (cand.SummaryForce < minForceNode.SummaryForce)
					minForceNode = cand;
			}

			return minForceNode;
		}
	}

	class Cluster
	{
		public Vector Center;
	}
}
