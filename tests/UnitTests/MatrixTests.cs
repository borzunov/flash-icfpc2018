using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure;
using Flash.Infrastructure.Models;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    public class MatrixTests
    {
        public static TestCaseData[] GetAdjacentCases =
        {
            new TestCaseData(new Matrix(3), new Vector(0, 0, 0), 3),
            new TestCaseData(new Matrix(3), new Vector(0, 0, 1), 4),
            new TestCaseData(new Matrix(3), new Vector(0, 1, 1), 5),
            new TestCaseData(new Matrix(3), new Vector(1, 1, 1), 6),
            new TestCaseData(new Matrix(3), new Vector(2, 2, 2), 3),
        };

        [TestCaseSource(nameof(GetAdjacentCases))]
        public void GetAdjacents_Correct(Matrix m, Vector v, int count)
        {
            m.GetAdjacents(v).Should().HaveCount(count);
        }

        public static TestCaseData[] GetNearsCases =
        {
            new TestCaseData(new Matrix(3), new Vector(0, 0, 0), 6),
            new TestCaseData(new Matrix(3), new Vector(0, 0, 1), 9),
            new TestCaseData(new Matrix(3), new Vector(0, 1, 1), 13),
            new TestCaseData(new Matrix(3), new Vector(1, 1, 1), 18),
            new TestCaseData(new Matrix(3), new Vector(2, 2, 2), 6),
        };

        [TestCaseSource(nameof(GetNearsCases))]
        public void GetNears_Correct(Matrix m, Vector v, int count)
        {
            m.GetNears(v).Should().HaveCount(count);
        }

        public static TestCaseData[] IsGroundedCases =
        {
            new TestCaseData(new Vector(2, 0, 2), true),
            new TestCaseData(new Vector(0, 1, 2), true),
            new TestCaseData(new Vector(0, 1, 0), true),
            new TestCaseData(new Vector(0, 2, 1), true),
            new TestCaseData(new Vector(2, 1, 0), false),
        };

        [TestCaseSource(nameof(IsGroundedCases))]
        public void IsGrounded_Correct(Vector v, bool expected)
        {
            var m = Matrix.Create(
                new[]
                {
                    @"
001
000
000",
                    @"
101
000
101",
                    @"
111
100
100"
                });
            m.IsGrounded(v).Should().Be(expected);
        }


    }
}
