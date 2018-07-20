using System.Collections.Generic;
using Flash.Infrastructure;
using Flash.Infrastructure.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    public class StateTests
    {
        [Test]
        public void IsValid_Fails_WhenNotGrounded()
        {
            var matrix = new Matrix(3);
            matrix.Fill(new Vector(1, 1, 1));
            var state = new State(0, false, matrix, new Bot[0], new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Succeeds_WhenNotGroundedButHarmonics()
        {
            var matrix = new Matrix(3);
            matrix.Fill(new Vector(1, 1, 1));
            var state = new State(0, true, matrix, new Bot[0], new Trace(new List<ICommand>()));
            state.IsValid().Should().BeTrue();
        }

        [Test]
        public void IsValid_Fails_WhenActiveBotsHaveSameIds()
        {
            var bots = new[] {new Bot(0, new Vector(0, 0, 0), new int[0]), new Bot(0, new Vector(1, 1, 1), new int[0])};
            var state = new State(0, false, new Matrix(2), bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Fails_WhenActiveBotsHaveSamePositions()
        {
            var bots = new[] {new Bot(0, new Vector(0, 0, 0), new int[0]), new Bot(1, new Vector(0, 0, 0), new int[0])};
            var state = new State(0, false, new Matrix(2), bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Fails_WhenActiveBotsIsFilled()
        {
            var matrix = new Matrix(3);
            matrix.Fill(new Vector(1, 1, 1));
            var bots = new[] {new Bot(0, new Vector(1, 1, 1), new int[0])};
            var state = new State(0, false, matrix, bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Fails_WhenSeedsAreNotDisjoint()
        {
            var bots = new[] {new Bot(0, new Vector(0, 0, 0), new []{2}), new Bot(1, new Vector(1, 1, 1), new []{2})};
            var state = new State(0, false, new Matrix(2), bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Fails_WhenActiveBotInSeed()
        {
            var bots = new[] {new Bot(0, new Vector(0, 0, 0), new []{1}), new Bot(1, new Vector(1, 1, 1), new []{0})};
            var state = new State(0, false, new Matrix(2), bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeFalse();
        }

        [Test]
        public void IsValid_Correct()
        {
            var matrix = new Matrix(3);
            matrix.Fill(new Vector(1, 0, 1));
            var bots = new[] {new Bot(0, new Vector(0, 0, 0), new []{2}), new Bot(1, new Vector(1, 1, 1), new []{3})};
            var state = new State(0, false, matrix, bots, new Trace(new List<ICommand>()));
            state.IsValid().Should().BeTrue();
        }
    }
}
