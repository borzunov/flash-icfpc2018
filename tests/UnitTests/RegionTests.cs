using Flash.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    public class RegionTests
    {
        public static TestCaseData[] CtorCases =
        {
            new TestCaseData(new Vector(0, 1, 0), new Vector(1, 0, 1), new Vector(0, 0, 0), new Vector(1, 1, 1)),
            new TestCaseData(new Vector(4, 5, 6), new Vector(1, 2, 3), new Vector(1, 2, 3), new Vector(4, 5, 6))
        };

        [TestCaseSource(nameof(CtorCases))]
        public void Ctor_Correct(Vector from, Vector to, Vector min, Vector max)
        {
            var region = new Region(from, to);

            region.Min.Should().Be(min);
            region.Max.Should().Be(max);
        }

        public static TestCaseData[] ContainsCases =
        {
            new TestCaseData(new Region(new Vector(1, 1, 1), new Vector(1, 1, 1)), new Vector(1, 1, 1), true),
            new TestCaseData(new Region(new Vector(0, 0, 0), new Vector(1, 1, 1)), new Vector(0, 0, 1), true),
            new TestCaseData(new Region(new Vector(0, 0, 0), new Vector(2, 2, 2)), new Vector(1, 1, 1), true),
            new TestCaseData(new Region(new Vector(0, 0, 0), new Vector(2, 2, 2)), new Vector(2, 2, 1), true),
            new TestCaseData(new Region(new Vector(0, 0, 0), new Vector(2, 2, 2)), new Vector(2, 1, 1), true),
            new TestCaseData(new Region(new Vector(0, 0, 0), new Vector(2, 2, 2)), new Vector(2, 2, 3), false),
        };

        [TestCaseSource(nameof(ContainsCases))]
        public void Contains_Correct(Region region, Vector v, bool result)
        {
            region.Contains(v).Should().Be(result);
        }


        public static TestCaseData[] DimCases =
        {
            new TestCaseData(new Region(new Vector(1, 1, 1), new Vector(1, 1, 1)), 0),
            new TestCaseData(new Region(new Vector(1, 1, 1), new Vector(1, 1, 2)), 1),
            new TestCaseData(new Region(new Vector(1, 1, 1), new Vector(2, 1, 2)), 2),
            new TestCaseData(new Region(new Vector(1, 1, 1), new Vector(2, 2, 2)), 3),
        };

        [TestCaseSource(nameof(DimCases))]
        public void Dim_Correct(Region region, int result)
        {
            region.Dim.Should().Be(result);
        }
    }
}
