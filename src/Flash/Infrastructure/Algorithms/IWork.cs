using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	interface IWork
	{
		Vector GetPossibleStartPlace(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate);

		Vector SetWorkerAndGetInput(IsGroundedChecker groundedChecker, Func<Vector, bool> isForbidden, Vector botCoordinate, int botId);

		bool IsEnoughWorkers();

		HashSet<Vector> GetWorkPlan();

		Dictionary<int, Vector> DoWork(Func<Vector, bool> isForbidden, out List<ICommand> commands, out List<Vector> vectors);
	}
}
