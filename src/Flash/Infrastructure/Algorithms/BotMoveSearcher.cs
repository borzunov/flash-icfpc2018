using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bingo.Graph;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class BotMoveSearcher
	{
		private Matrix Matrix;
		private IsGroundedChecker GroundedChecker;
		private Vector BotPosition;
		private Vector End;
		private Func<Vector, bool> IsForbiddenArea;
		private int BotsCount;

		private Dictionary<Tuple<Vector, bool>, AStarState> MinStates = new Dictionary<Tuple<Vector, bool>, AStarState>();

		public BotMoveSearcher(Matrix matrix, Vector botPosition, Func<Vector, bool> isForbiddenArea, int botsCount, Vector end)
		{
			Matrix = matrix;
			BotPosition = botPosition;
			IsForbiddenArea = isForbiddenArea;
			BotsCount = botsCount;
			End = end;
		}

		public bool FindPath(out List<Vector> movePositions, out List<ICommand> commands)
		{
			var priorityQueue = new PriorityQueue<AStarState>();
			var startState = new AStarState
			{
				EndPosition = BotPosition
			};
			priorityQueue.Enqueue(0, startState);
			MinStates[Tuple.Create(startState.EndPosition, false)] = startState;

			bool isEnd = false;
			AStarState result = null;

			while (!priorityQueue.Empty)
			{
				var state = priorityQueue.Dequeue();
				
				foreach (var newState in GetSJumps(state).Concat(GetLJumps(state)).Concat(GetJumpsIntoFills(state)))
				{
					if(MinStates.TryGetValue(Tuple.Create(newState.EndPosition, newState.DestroyedCell != null), out var oldState) && oldState.Weight <= newState.Weight)
						continue;

					if (state.EndPosition == End)
					{
						result = state;
						isEnd = true;
						break;
					}

					MinStates[Tuple.Create(newState.EndPosition, newState.DestroyedCell != null)] = newState;
					priorityQueue.Enqueue(newState.MaxPotentialWeight, newState);
				}

				if(isEnd)
					break;
			}

			if (!isEnd)
			{
				movePositions = null;
				commands = null;
				return false;
			}

			var states = result.GetStates().Reverse().ToList();
			commands = states.SelectMany(state => state.GetCommands()).ToList();
			movePositions = states.SelectMany(state => state.GetUsedVectors()).ToList();
			return true;
		}

		public IEnumerable<AStarState> GetSJumps(AStarState botState)
		{
			var botPosition = botState.EndPosition;
			var currentWeight = botState.Weight;

			foreach (var vector in new Vector(0, 0, 0).GetAdjacents())
			{
				for (int i = 1; i <= 15; i++)
				{
					if(botState.DestroyedCell != null && i > 1)
						break;

					var move = vector * i;
					var position = botPosition + move;
					if(Matrix.IsFull(position) || IsForbiddenArea(position))
						break;
					var weight = currentWeight + GetSMoveWeight(i);
					yield return new AStarState
					{
						LastDestroyedCell = botState.DestroyedCell,
						Move1 = move,
						Dad = botState,
						EndPosition = position,
						Weight = weight,
						MaxPotentialWeight = weight + GetPotentialOptimalWeight(position, End)
					};
				}
			}
		}

		public IEnumerable<AStarState> GetLJumps(AStarState botState)
		{
			if (botState.DestroyedCell!= null)
				yield break;

			var botPosition = botState.EndPosition;
			var currentWeight = botState.Weight;

			var moveVectors = new[]
			{
				new Vector(1, 0, 0),
				new Vector(0, 1, 0),
				new Vector(0, 0, 1)
			};
			
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if(i == j)
						continue;
					for (int x = -5; x <= 5; x++)
					{
						if (x == 0)
							continue;

						var move1 = moveVectors[i] * x;
						if (Matrix.IsFull(botPosition + move1) || IsForbiddenArea(botPosition + move1))
							if(x < 0)
								continue;
							else
								break;
						
						for (int y = -5; y <= 5; y++)
						{
							if(y == 0)
								continue;

							var move2 = moveVectors[j] * y;
							if (Matrix.IsFull(botPosition + move1 + move2) || IsForbiddenArea(botPosition + move1 + move2))
								if (y < 0)
									continue;
								else
									break;
							
							var position = botPosition + move1 + move2;
							if (Matrix.IsFull(position) || IsForbiddenArea(position))
								break;
							var weight = currentWeight + GetLMoveWeight(x, y);
							yield return new AStarState
							{
								Move1 = move1,
								Move2 = move2,
								Dad = botState,
								EndPosition = position,
								Weight = weight,
								MaxPotentialWeight = weight + GetPotentialOptimalWeight(position, End)
							};
						}
					}
				}
			}
		}

		public IEnumerable<AStarState> GetJumpsIntoFills(AStarState botState)
		{
			var currentWeight = botState.Weight;
			foreach (var adjacent in botState.EndPosition.GetAdjacents())
			{
				if(Matrix.IsVoid(adjacent))
					continue;
				var weight = currentWeight + GetInFillWeight();

				if(!GroundedChecker.CanRemove(new[] {adjacent, botState.DestroyedCell}.Where(c => c != null).ToList()))
					continue;

				yield return new AStarState
				{
					Move1 = adjacent - botState.EndPosition,
					EndPosition = adjacent,
					DestroyedCell = adjacent,
					LastDestroyedCell = botState.DestroyedCell,
					Weight = weight,
					Dad = botState,
					MaxPotentialWeight = weight + GetPotentialOptimalWeight(adjacent, End)
				};
			}

		}

		public long GetSMoveWeight(int distance)
		{
			return distance * 2 + 3 * Matrix.R * Matrix.R * Matrix.R / BotsCount;
		}

		public long GetLMoveWeight(int distance1, int distance2)
		{
			return distance1 * 2 + distance2 * 2 + 4 + 3 * Matrix.R * Matrix.R * Matrix.R / BotsCount;
		}

		public long GetInFillWeight()
		{
			return 2 + 4 + 9 * Matrix.R * Matrix.R * Matrix.R / BotsCount;
		}

		public long GetPotentialOptimalWeight(Vector start, Vector end)
		{
			var move = start - end;
			var axisesCount = AxisesCount(move);
			var movesCount = axisesCount + (move.X + move.Y + move.Z - 10 * axisesCount) / 15;
			return axisesCount * 2 + move.X * 2 + move.Y * 2 + move.Z * 2 + 3 * (long)Matrix.R * Matrix.R * Matrix.R * movesCount / BotsCount;
		}

		private int AxisesCount(Vector vector)
		{
			return (vector.X > 0 ? 1 : 0) + (vector.Y > 0 ? 1 : 0) + (vector.Z > 0 ? 1 : 0);
		}
	}

	public class AStarState
	{
		public Vector LastDestroyedCell;
		public Vector DestroyedCell;
		public Vector Move1;
		public Vector Move2;
		public Vector StartPosition;
		public Vector EndPosition;
		public AStarState Dad;
		public long Weight;
		public long MaxPotentialWeight;

		public IEnumerable<ICommand> GetCommands()
		{
			if (DestroyedCell != null)
				yield return new FillCommand(EndPosition - StartPosition);
			if (Move1 != null && Move2 == null)
				yield return new SMoveCommand(Move1);
			if (Move1 != null && Move2 != null)
				yield return new LMoveCommand(Move1, Move2);
			if (LastDestroyedCell != null)
				yield return new FillCommand(LastDestroyedCell - EndPosition);
		}

		public IEnumerable<Vector> GetUsedVectors()
		{
			var pos = StartPosition;

			var normalizedMove1 = Move1.Normalize();
			for (int i = 1; i <= Move1.Mlen; i++)
			{
				pos += normalizedMove1;
				yield return pos;
			}

			if(Move2 == null)
				yield break;

			var normalizedMove2 = Move2.Normalize();
			for (int i = 1; i <= Move2.Mlen; i++)
			{
				pos += normalizedMove2;
				yield return pos;
			}
		}

		public IEnumerable<AStarState> GetStates()
		{
			var node = this;
			while (node != null)
			{
				yield return node;
				node = node.Dad;
			}
		}
	}
}
