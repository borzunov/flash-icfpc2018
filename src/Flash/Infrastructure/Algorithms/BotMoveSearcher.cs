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
		private Matrix Model;
		private IsGroundedChecker GroundedChecker;
		private Vector BotPosition;
		private Vector End;
		private Func<Vector, bool> IsForbiddenArea;
		private int BotsCount;
		public JsonOpLogWriter mongoOplogWriter;

		private Dictionary<Tuple<Vector, bool>, AStarState> MinStates = new Dictionary<Tuple<Vector, bool>, AStarState>();

		public BotMoveSearcher(Matrix matrix, Matrix model, Vector botPosition, Func<Vector, bool> isForbiddenArea, int botsCount, Vector end, IsGroundedChecker groundedChecker)
		{
			Matrix = matrix;
			BotPosition = botPosition;
			IsForbiddenArea = isForbiddenArea;
			BotsCount = botsCount;
			End = end;
			GroundedChecker = groundedChecker;
			Model = model;
		}
		private static Random rand = new Random();
		public bool FindPath(out List<Vector> movePositions, out List<ICommand> commands, out int iterationsCount)
		{
			var priorityQueue = new PriorityQueue<AStarState>();
			var startState = new AStarState
			{
				DestroyedCell = Model.IsFull(BotPosition) ? BotPosition : null,
				EndPosition = BotPosition,
				Straight = true
			};
			var endState = new AStarState
			{
				EndPosition = End,
				StartPosition = End,
				Straight = false,
				DestroyedCell = Matrix.IsFull(End) ? End : null
			};
			priorityQueue.Enqueue(0, startState);
			priorityQueue.Enqueue(0, endState);
			MinStates[Tuple.Create(startState.EndPosition, false)] = startState;
			MinStates[Tuple.Create(endState.EndPosition, false)] = endState;

			bool isEnd = false;
			AStarState result = null;
			AStarState endResult = null;

			iterationsCount = 0;
			var bestWeight = long.MaxValue;

			while (!priorityQueue.Empty)
			{
				var state = priorityQueue.Dequeue();
				if(MinStates.TryGetValue(Tuple.Create(state.EndPosition, state.DestroyedCell != null), out var minState) && minState.Weight < state.Weight)
					continue;
				
				foreach (var newState in GetSJumps(state).Concat(GetLJumps(state)).Concat(GetJumpsIntoFills(state)))
				{
					if(MinStates.TryGetValue(Tuple.Create(newState.EndPosition, newState.DestroyedCell != null), out var oldState) && oldState.Weight <= newState.Weight && newState.Straight == oldState.Straight)
						continue;

					if (oldState != null && newState.Straight != oldState.Straight)
					{
						if (newState.Straight)
						{
							result = newState;
							endResult = oldState;
						}
						else
						{
							result = oldState;
							endResult = newState;
						}

						if (newState.DestroyedCell == null)
						{
							isEnd = true;
							break;
						}
					}

					MinStates[Tuple.Create(newState.EndPosition, newState.DestroyedCell != null)] = newState;
					priorityQueue.Enqueue(-newState.MaxPotentialWeight + (newState.Straight 
						                      ? (End - newState.EndPosition).Euclidlen 
						                      : (BotPosition - newState.EndPosition).Euclidlen ) / Matrix.R / 3, newState);
					iterationsCount++;
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

			var states = result.GetStates().Reverse().Concat(endResult.GetStates()).ToList();
			commands = states.SelectMany(state => state.GetCommands()).ToList();
			movePositions = states.SelectMany(state => state.Straight ? state.GetUsedVectors() : state.GetUsedVectors().Reverse()).ToList();
			
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
					if(!Matrix.Contains(position) || Matrix.IsFull(position) || IsForbiddenArea(position) || position == BotPosition)
						break;
					var weight = currentWeight + GetSMoveWeight(i);
					yield return new AStarState
					{
						LastDestroyedCell = botState.DestroyedCell,
						Move1 = move,
						Dad = botState,
						StartPosition = botState.EndPosition,
						EndPosition = position,
						Weight = weight,
						MaxPotentialWeight = weight + GetPotentialOptimalWeight(position, botState.Straight),
						Straight = botState.Straight
					};
				}
			}
		}

		public IEnumerable<AStarState> GetLJumps(AStarState botState)
		{
			var ss = new[] {-1, 1, -2, 2, -3, 3, -4, 4, -5, 5};

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
					bool bannedNX = false;
					bool bannedPX = false;

					foreach (var x in ss)
					{
						var move1 = moveVectors[i] * x;
						if(x < 0 && bannedNX || x > 0 && bannedPX)
							continue;
						if (!Matrix.Contains(botPosition + move1) || Matrix.IsFull(botPosition + move1) ||
						    IsForbiddenArea(botPosition + move1) || botPosition + move1 == BotPosition)
						{
							if (x < 0)
								bannedNX = true;
							else
								bannedPX = true;
							continue;
						}

						bool bannedNY = false;
						bool bannedPY = false;

						foreach (var y in ss)
						{
							var move2 = moveVectors[j] * y;
							if (y < 0 && bannedNY || y > 0 && bannedPY)
								continue;
							if (!Matrix.Contains(botPosition + move1 + move2) || Matrix.IsFull(botPosition + move1 + move2) ||
							    IsForbiddenArea(botPosition + move1 + move2) || botPosition + move1 + move2 == BotPosition)
							{
								if (y < 0)
									bannedNY = true;
								else
									bannedPY = true;
								continue;
							}
							
							var position = botPosition + move1 + move2;
							var weight = currentWeight + GetLMoveWeight(x, y);
							var state = new AStarState
							{
								Move1 = move1,
								Move2 = move2,
								Dad = botState,
								StartPosition = botState.EndPosition,
								EndPosition = position,
								Weight = weight,
								MaxPotentialWeight = weight + GetPotentialOptimalWeight(position, botState.Straight),
								Straight = botState.Straight
							};
							
							yield return state;
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
				if(!Matrix.Contains(adjacent) || Matrix.IsVoid(adjacent))
					continue;
				var weight = currentWeight + GetInFillWeight();
				
				if (!GroundedChecker.CanRemove(new[] { adjacent, botState.DestroyedCell }.Where(c => c != null).ToHashSet()))
					continue;

				yield return new AStarState
				{
					Move1 = adjacent - botState.EndPosition,
					StartPosition = botState.EndPosition,
					EndPosition = adjacent,
					DestroyedCell = adjacent,
					LastDestroyedCell = botState.DestroyedCell,
					Weight = weight,
					Dad = botState,
					MaxPotentialWeight = weight + GetPotentialOptimalWeight(adjacent, botState.Straight),
					Straight = botState.Straight
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

		public long GetPotentialOptimalWeight(Vector start, bool straight)
		{
			var move = start - (straight ? End : BotPosition); 
			move = move.Abs();
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

		public bool Straight;

		public IEnumerable<ICommand> GetCommands()
		{
			if (Straight)
			{
				if (DestroyedCell != null && Move1 != null)
					yield return new VoidCommand(EndPosition - StartPosition, EndPosition);
				if (Move1 != null && Move2 == null)
					yield return new SMoveCommand(Move1);
				if (Move1 != null && Move2 != null)
					yield return new LMoveCommand(Move1, Move2);
				if (LastDestroyedCell != null)
					yield return new FillCommand(LastDestroyedCell - EndPosition, LastDestroyedCell);
			}
			else
			{
				if (LastDestroyedCell != null)
					yield return new VoidCommand(LastDestroyedCell - EndPosition, LastDestroyedCell);
				if (Move1 != null && Move2 == null)
					yield return new SMoveCommand(-Move1);
				if (Move1 != null && Move2 != null)
					yield return new LMoveCommand(-Move2, -Move1);
				if (DestroyedCell != null && StartPosition != null)
					yield return new FillCommand(DestroyedCell - StartPosition, EndPosition);
			}
		}

		public IEnumerable<Vector> GetUsedVectors()
		{
			var pos = StartPosition;
			if (Move1 != null && !Straight)
				yield return StartPosition;

			if (Move1 == null)
				yield break;
			var normalizedMove1 = Move1.Normalize();
			for (int i = 1; i <= Move1.Mlen; i++)
			{
				pos += normalizedMove1;
				yield return pos;
			}

			if (Move2 == null)
				yield break;

			var normalizedMove2 = Move2.Normalize();
			for (int i = 1; i < Move2.Mlen; i++)
			{
				pos += normalizedMove2;
				yield return pos;
			}

			if (Straight)
				yield return EndPosition;
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

		public override string ToString()
		{
			return Straight.ToString();
		}
	}
}
