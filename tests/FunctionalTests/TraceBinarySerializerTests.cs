using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Serializers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FunctionalTests
{
    public class TraceBinarySerializerTests
    {
        private readonly TraceBinarySerializer traceBinarySerializer = new TraceBinarySerializer();

        private static IEnumerable<TestCaseData> GetSerializeTest()
        {
            yield return new TestCaseData(new Trace(new []{new HaltCommand()}), new byte[]{0b1111_1111})
                .SetName("Halt serialization");

        }

        [TestCaseSource(nameof(GetSerializeTest))]
        public void Serialize_ShouldReturnRigthByteSequence(Trace trace, byte[] expectedBytes)
        {
            var sut = traceBinarySerializer.Serialize(trace);

            sut.Should().BeEquivalentTo(expectedBytes, op => op.WithStrictOrdering());
        }
    }
}
