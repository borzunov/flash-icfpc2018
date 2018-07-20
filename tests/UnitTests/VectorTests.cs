using Flash.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    public class VectorTests
    {
        [TestCase(1, 2, 3, ExpectedResult = 3)]
        [TestCase(3, 1, 2, ExpectedResult = 3)]
        [TestCase(3, 2, 1, ExpectedResult = 3)]
        [TestCase(-2, 1, 0, ExpectedResult = 2)]
        [TestCase(-2, -3, 1, ExpectedResult = 3)]
        [TestCase(0, 0, 1, ExpectedResult = 1)]
        [TestCase(0, 1, 0, ExpectedResult = 1)]
        [TestCase(1, 0, 0, ExpectedResult = 1)]
        public int Clen_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).Clen;
        }

        [TestCase(1, 1, 1, ExpectedResult = 3)]
        [TestCase(-1, -1, -1, ExpectedResult = 3)]
        [TestCase(0, 0, 0, ExpectedResult = 0)]
        [TestCase(0, 0, 1, ExpectedResult = 1)]
        [TestCase(0, 1, 0, ExpectedResult = 1)]
        [TestCase(1, 0, 0, ExpectedResult = 1)]
        [TestCase(1, 2, -3, ExpectedResult = 6)]
        public int Mlen_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).Mlen;
        }

        public static TestCaseData[] SumCases =
        {
            new TestCaseData(new Vector(1, 1, 1), new Vector(1, 1, 1), new Vector(2, 2, 2)),
            new TestCaseData(new Vector(1, 2, 3), new Vector(-1, -2, -3), new Vector(0, 0, 0)),
            new TestCaseData(new Vector(0, 0, 1), new Vector(1, 0, 0), new Vector(1, 0, 1)),
        };

        [TestCaseSource(nameof(SumCases))]
        public void Sum_Correct(Vector v1, Vector v2, Vector result)
        {
            (v1 + v2).Should().Be(result);
        }



        public static TestCaseData[] DiffCases =
        {
            new TestCaseData(new Vector(1, 1, 1), new Vector(1, 1, 1), new Vector(0, 0, 0)),
            new TestCaseData(new Vector(1, 2, 3), new Vector(-1, -2, -3), new Vector(2, 4, 6)),
            new TestCaseData(new Vector(0, 0, 1), new Vector(1, 0, 0), new Vector(-1, 0, 1)),
        };

        [TestCaseSource(nameof(DiffCases))]
        public void Diff_Correct(Vector v1, Vector v2, Vector result)
        {
            (v1 - v2).Should().Be(result);
        }

        public static TestCaseData[] AdjacentCases =
        {
            new TestCaseData(new Vector(0, 0, 0), new Vector(0, 0, 1), true),
            new TestCaseData(new Vector(0, 0, 0), new Vector(0, 0, 0), false),
            new TestCaseData(new Vector(0, 0, 0), new Vector(10, 1, 0), false),
            new TestCaseData(new Vector(0, 0, 0), new Vector(1, 1, 0), false),
            new TestCaseData(new Vector(0, 0, 0), new Vector(1, 1, 1), false),
            new TestCaseData(new Vector(1, 2, 3), new Vector(1, 1, 3), true),
            new TestCaseData(new Vector(1, 2, 3), new Vector(1, 2, 1), false),
            new TestCaseData(new Vector(-1, -2, -3), new Vector(-1, -2, -4), true)
        };

        [TestCaseSource(nameof(AdjacentCases))]
        public void Adjacent_Correct(Vector v1, Vector v2, bool result)
        {
            v1.IsAdjacentTo(v2).Should().Be(result);
        }

        [TestCase(0, 0, 1, ExpectedResult = true)]
        [TestCase(0, 1, 0, ExpectedResult = true)]
        [TestCase(1, 0, 0, ExpectedResult = true)]
        [TestCase(-1, 0, 0, ExpectedResult = true)]
        [TestCase(-5, 0, 0, ExpectedResult = true)]
        [TestCase(0, 0, 0, ExpectedResult = false)]
        [TestCase(1, 2, 0, ExpectedResult = false)]
        public bool IsLd_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).IsLd;
        }


        [TestCase(1, 0, 0, ExpectedResult = true)]
        [TestCase(5, 0, 0, ExpectedResult = true)]
        [TestCase(6, 0, 0, ExpectedResult = false)]
        [TestCase(1, 1, 0, ExpectedResult = false)]
        public bool IsSld_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).IsSld;
        }

        [TestCase(1, 0, 0, ExpectedResult = true)]
        [TestCase(15, 0, 0, ExpectedResult = true)]
        [TestCase(16, 0, 0, ExpectedResult = false)]
        [TestCase(1, 1, 0, ExpectedResult = false)]
        public bool IsLld_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).IsLld;
        }

        [TestCase(1, 0, 0, ExpectedResult = true)]
        [TestCase(1, 1, 0, ExpectedResult = true)]
        [TestCase(1, 1, 1, ExpectedResult = false)]
        [TestCase(0, 0, 0, ExpectedResult = false)]
        public bool IsNd_Correct(int x, int y, int z)
        {
            return new Vector(x, y, z).IsNd;
        }
    }
}
