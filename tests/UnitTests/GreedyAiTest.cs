using Flash.Infrastructure;
using Flash.Infrastructure.Models;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
	public class GreedyAiTest
	{
		[Test]
		public void NAME()
		{
			var matrix = new bool[12, 12, 12];
			matrix[0, 0, 6] = true;
			var resultMatrix = new Matrix(matrix);
			var vector = GreedyAI.bfs(resultMatrix, new Bot(1, new Vector(0, 0, 0), new int[] {12}), State.CreateInitial(12));
			vector.Should().Be(new Vector(0, 0, 6));
		}
	}
}