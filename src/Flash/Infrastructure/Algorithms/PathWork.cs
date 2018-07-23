using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class PathWork : IWork
	{
		private Vector BotPosition;
		private Vector EndPosition;
		private Matrix matrix;
		private Matrix model;
		private IsGroundedChecker isGroundedChecker;
		private int BotsCount;
		public JsonOpLogWriter mongoOplogWriter;
		public int BotId;

		public IEnumerable<int> GetBots()
		{
			yield return BotId;
		}

		public PathWork(Vector botPosition, Vector endPosition, Matrix matrix, IsGroundedChecker isGroundedChecker, int botsCount, int botId, Matrix model)
		{
			BotPosition = botPosition;
			EndPosition = endPosition;
			this.matrix = matrix;
			this.isGroundedChecker = isGroundedChecker;
			BotsCount = botsCount;
			BotId = botId;
			this.model = model;
		}

		public Vector GetPossibleStartPlace(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate)
		{
			throw new NotImplementedException();
		}

		public Vector SetWorkerAndGetInput(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate, int botId)
		{
			throw new NotImplementedException();
		}

		public bool IsEnoughWorkers()
		{
			return true;
		}

		public HashSet<Vector> GetWorkPlan()
		{
			return new HashSet<Vector>();
		}

		public Dictionary<int, Vector> DoWork(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, out List<ICommand> commands, out List<Vector> vectors)
		{
			var searcher = new BotMoveSearcher(matrix, model, BotPosition, isForbidden, BotsCount, EndPosition, isGroundedChecker){mongoOplogWriter = mongoOplogWriter};
			searcher.FindPath(out vectors, out commands, out _);
			return new Dictionary<int, Vector>
			{
				{ BotId, EndPosition }
			};
		}
	}
}
