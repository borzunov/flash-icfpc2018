using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Serializers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FunctionalTests
{
    public class TraceBinarySerializerTests
    {
        private readonly TraceBinarySerializer traceBinarySerializer = TraceBinarySerializer.Create();

        private static IEnumerable<TestCaseData> GetSerializeTest()
        {
            yield return new TestCaseData(Trace.From(new HaltCommand()), new byte[]{0b1111_1111}).SetName("Halt");

            yield return new TestCaseData(Trace.From(new WaitCommand()), new byte[]{0b1111_1110}).SetName("Wait");

            yield return new TestCaseData(Trace.From(new FlipCommand()), new byte[]{0b1111_1101}).SetName("Flip");

            yield return new TestCaseData(
                Trace.From(new SMoveCommand(new Vector(12, 0, 0))),
                new byte[]{0b0001_0100, 0b0001_1011})
                .SetName("SMove");

            yield return new TestCaseData(
                Trace.From(new SMoveCommand(new Vector(0, 0, -4))),
                new byte[] { 0b0011_0100, 0b0000_1011 })
                .SetName("SMove with negative");

            yield return new TestCaseData(
                Trace.From(new LMoveCommand(new Vector(3, 0, 0), new Vector(0, -5, 0))),
                new byte[]{ 0b1001_1100, 0b0000_1000})
                .SetName("LMove 1");

            yield return new TestCaseData(
                    Trace.From(new LMoveCommand(new Vector(0, -2, 0), new Vector(0, 0, 2))),
                    new byte[] { 0b1110_1100, 0b0111_0011 })
                .SetName("LMove 2");

            yield return new TestCaseData(
                    Trace.From(new FusionPCommand(new Vector(-1, 1, 0))),
                    new byte[] { 0b0011_1111 })
                .SetName("FusionP");

            yield return new TestCaseData(
                    Trace.From(new FusionSCommand(new Vector(1, -1, 0))),
                    new byte[] { 0b1001_1110 })
                .SetName("FusionS");

            yield return new TestCaseData(
                    Trace.From(new FissionCommand(new Vector(0, 0, 1), 5)),
                    new byte[] { 0b0111_0101, 0b0000_0101 })
                .SetName("Fission");

            yield return new TestCaseData(
                    Trace.From(new FillCommand(new Vector(0, -1, 0))),
                    new byte[] { 0b0101_0011 })
                .SetName("Fill");
        }

        [TestCaseSource(nameof(GetSerializeTest))]
        public void Serialize_ShouldReturnRigthByteSequence(Trace trace, byte[] expectedBytes)
        {
            var sut = traceBinarySerializer.Serialize(trace);

            sut.Should().BeEquivalentTo(expectedBytes, op => op.WithStrictOrdering());
        }
    }
}
