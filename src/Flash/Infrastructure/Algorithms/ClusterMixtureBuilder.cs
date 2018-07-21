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

		public ClusterMixtureBuilder(State state)
		{
			R = state.Matrix.R;

			BotsCount = state.Bots.Sum(bot => 1 + bot.Seeds.Length);
			Clusters = new Cluster[BotsCount];
			Weights = new double[state.Matrix.R, state.Matrix.R, state.Matrix.R][];
			NormalizedWeights = new double[state.Matrix.R, state.Matrix.R, state.Matrix.R][];
			
			var voxels = new List<Vector>();

			for (int x = 0; x < state.Matrix.R; x++)
			{
				for (int y = 0; y < state.Matrix.R; y++)
				{
					for (int z = 0; z < state.Matrix.R; z++)
					{
						if (state.Matrix.IsFull(new Vector(x, y, z)))
						{
							var array = Weights[x, y, z] = Clusters.Select(c => 0.0).ToArray();
							NormalizedWeights[x, y, z] = Clusters.Select(c => 0.0).ToArray();
							Distances[x,y,z] = Clusters.Select(c => 0.0).ToArray();

							var sum = array.Sum();
							for (int i = 0; i < array.Length; i++)
							{
								array[i] /= sum;
							}
							voxels.Add(new Vector(x, y, z));
						}
					}
				}

				Voxels = voxels.ToHashSet();
			}

			var rand = new Random();

			foreach (var cluster in Clusters)
			{
				var newCenter = voxels[rand.Next(voxels.Count)];
				while(Clusters.Any(cluster1 => cluster1.Center.Equals(newCenter)))
					newCenter = voxels[rand.Next(voxels.Count)];
				cluster.Center = newCenter;
			}

			ClustersSizes = Clusters.Select(cluster => voxels.Count / Clusters.Length).ToArray();
		}

		public TreeNode[] ClusterRoots;

		public void CountWeights()
		{
			Random rand = new Random();

			for (int i = 0; i < Clusters.Length; i++)
			{
				var distCount = new Dictionary<int, int>();
				var count = ClustersSizes[i];

				ClusterRoots[i] = new TreeNode(Clusters[i].Center);

				var usedPoints = new HashSet<Vector>();
				var queue = new Queue<Tuple<Vector, int, TreeNode>>();
				queue.Enqueue(Tuple.Create(Clusters[i].Center, 0, ClusterRoots[i]));
				usedPoints.Add(Clusters[i].Center);

				while (queue.Count > 0)
				{
					var node = queue.Dequeue();
					count--;
					distCount[node.Item2] = count;
					Weights[node.Item1.X, node.Item1.Y, node.Item1.Z][i] =
						node.Item2 == 0 
							? count 
							: distCount[node.Item2 - 1] / node.Item2;

					foreach (var near in node.Item1.GetAdjacents()
						.OrderBy(_ => rand.NextDouble())
						.Where(near => !usedPoints.Contains(near) && Voxels.Contains(near)))
					{

						queue.Enqueue(Tuple.Create(near, node.Item2 + 1, node.Item3.AddChild(near)));
						usedPoints.Add(node.Item1);
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
			for (var index = 0; index < ClusterRoots.Length; index++)
			{
				var clusterRoot = ClusterRoots[index];
				clusterRoot.UpdateForces(NormalizedWeights, index);
				clusterRoot.UpdateSummaryForces(0);
				Clusters[index].Center = clusterRoot.GetMinimumForceNode().Node;
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
			for (int i = 0; i < 20; i++)
			{
				CountWeights();
				NormalizeWeights();
				Console.WriteLine(CountMetric());
				var res = GetComponents();
				for (int j = 0; j < Clusters.Length; j++)
				{
					Console.WriteLine($"coponent {j}: {res[j].Count} nodes");
				}
				UpdateCenters();
			}

			return GetComponents();
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
			var minForce = dadForce;
			foreach (var child in Children)
			{
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
