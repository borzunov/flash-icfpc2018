using System.Collections.Generic;
using Flash.Infrastructure;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    [Ignore("INVESTIGATE ME")]
    [TestFixture]
    class MatrixDesirializerTests
    {
        private static IEnumerable<TestCaseData> GetTests()
        {
            var matrix = new bool[2,2,2];
            matrix[0, 0, 1] = true;
            yield return new TestCaseData(new byte[]{0b00000010, 0b01000000 },new Matrix(matrix));
        }

        [TestCaseSource(nameof(GetTests))]
        public void Deserialize_ReturnsExpectedMatrix(byte[] bytes, Matrix expectedMatrix)
        {
            var sut = MatrixDeserializer.Deserialize(bytes);

            sut.Should().BeEquivalentTo(expectedMatrix, op => op.RespectingRuntimeTypes());
        }
    }
}