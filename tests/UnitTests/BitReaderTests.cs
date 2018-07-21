using System.Collections.Generic;
using Flash.Infrastructure.Tools;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class BitReaderTests
    {
        [TestCaseSource(nameof(GetTestData))]
        public void BitReader(byte[] bytes, int index, int expectedBit)
        {
            var bitReader = new BitReader(bytes);
            bitReader.GetBit(index).Should().Be(expectedBit);
        }

        public static IEnumerable<TestCaseData> GetTestData()
        {
            yield return new TestCaseData(new byte[] { 0b00000001 }, 0, 0);
            yield return new TestCaseData(new byte[] { 0b00000001 }, 7, 1);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b00100001 }, 10, 1);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b00100001 }, 11, 0);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b00100001 }, 8, 0);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b10100001 }, 8, 1);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b10100001 }, 15, 1);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b10100001 }, 15, 1);
            yield return new TestCaseData(new byte[] { 0b00000001, 0b00100001, 0b00100001 }, 18, 1);
        }
    }
}
