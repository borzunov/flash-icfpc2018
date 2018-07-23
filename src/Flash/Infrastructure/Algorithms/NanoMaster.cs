//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Flash.Infrastructure.Commands;
//using Flash.Infrastructure.Models;
//using Flash.Infrastructure.Simulation;

//namespace Flash.Infrastructure.Algorithms
//{
//	class NanoMaster : ISolver
//	{
//		private Matrix tgtMatrix; 

//		private Simulator simulator;
//		private State state;
//		private IsGroundedChecker groundedChecker;
//		private List<int> bots = new List<int>();
//		private Dictionary<int, IWork> currentWorks = new Dictionary<int, IWork>();
//		private Dictionary<IWork, HashSet<int>> botsWorks = new Dictionary<IWork, HashSet<int>>();
//		private Dictionary<int, Queue<Tuple<ICommand, IWork>>> tasks = new Dictionary<int, Queue<Tuple<ICommand, IWork>>>();

//		private Dictionary<IWork, List<Vector>> Blocked = new Dictionary<IWork, List<Vector>>();
//		private Dictionary<Vector, IWork> BlockedVectors = new Dictionary<Vector, IWork>();
		
//		public Trace Solve(Matrix srcMatrix, Matrix tgtMatrix)
//		{
//			this.tgtMatrix = tgtMatrix;

//			Init(srcMatrix, tgtMatrix);

//			var figureToAdd = new HashSet<Vector>();
//			var figureToRemove = new HashSet<Vector>();
			
//			for (var x = 0; x < srcMatrix.R; x++)
//			for (var y = 0; y < srcMatrix.R; y++)
//			for (var z = 0; z < srcMatrix.R; z++)
//			{
//				var point = new Vector(x, y, z);
//				if (srcMatrix.IsVoid(point) && srcMatrix.IsFull(point))
//				{
//					figureToAdd.Add(point);
//				}
//				if (srcMatrix.IsFull(point) && srcMatrix.IsVoid(point))
//				{
//					figureToRemove.Add(point);
//				}
//			}

//			while (figureToAdd.Any())
//			{
//				foreach (var bot in bots)
//				{
//					if(bot)
//				}
//			}
//		}

//		private bool StartWork(IWork work)
//		{
//			if (work.IsEnoughWorkers())
//				return false;
//			var path = new PathWork(new Vector(0, 0, 0),
//				work.SetWorkerAndGetInput(groundedChecker, vector => false, new Vector(0, 0, 0), 0), state.Matrix, groundedChecker, 29, 0, tgtMatrix);

//			var commands = new List<Tuple<ICommand, IWork>>();
//			var vectors = new List<Vector>();

//			path.DoWork(groundedChecker, v => BlockedVectors.ContainsKey(v) && BlockedVectors[v] != path, out var cmds,
//				out var vctrs);

//			foreach (var command in cmds)
//			{
//				commands.Add(Tuple.Create(command, (IWork)path));
//			}
//			vectors.AddRange(vctrs);

//			work.DoWork(groundedChecker, v => BlockedVectors.ContainsKey(v) && BlockedVectors[v] != path, out cmds,
//				out vctrs);

//			foreach (var bot in path.GetBots())
//			{
//				Block(bot, path, vectors);
//			}

//			vectors.Clear();

//			foreach (var command in cmds)
//			{
//				commands.Add(Tuple.Create(command, (IWork)path));
//			}

//			vectors.AddRange(vctrs);

//			foreach (var bot in path.GetBots())
//			{
//				Block(bot, work, vectors);
//			}


//			foreach (var bot in path.GetBots())
//			{
//				if (!tasks.TryGetValue(bot, out var queue))
//				{
//					tasks[bot] = queue = new Queue<Tuple<ICommand, IWork>>();
//				}

//				foreach (var command in commands)
//				{
//					queue.Enqueue(command);
//				}
//			}
			
//			return true;
//		}

//		private void Block(int bot, IWork work, List<Vector> vectors)
//		{
//			currentWorks[bot] = work;
//			if(!botsWorks.ContainsKey(work))
//				botsWorks[work] = new HashSet<int>();
//			botsWorks[work].Add(bot);
//			Blocked[work] = vectors;
//			vectors.ForEach(v => BlockedVectors[v] = work);
//		}

//		private void UnBlockIfHaveTo(int bot)
//		{
//			if (!currentWorks.ContainsKey(bot))
//				return;
//			var work = currentWorks[bot];
//			if (tasks[bot].Count != 0 && tasks[bot].Peek().Item2 == work)
//				return;
//			botsWorks[work].Remove(bot);
//			if(botsWorks[work].Count > 0)
//				return;
//			currentWorks[bot] = tasks[bot].Count > 0 ? tasks[bot].Peek().Item2 : null;
//			botsWorks.Remove(work);
//			Blocked[work].ForEach(v => BlockedVectors.Remove(v));
//			Blocked.Remove(work);
//		}

//		private void Init(Matrix srcMatrix, Matrix tgtMatrix)
//		{
//			state = State.CreateInitial(srcMatrix.R, new FakeOpLog());
//			simulator = new Simulator();
//			groundedChecker = new IsGroundedChecker(srcMatrix);
//			bots.Add(0);
//		}
//	}
//}
