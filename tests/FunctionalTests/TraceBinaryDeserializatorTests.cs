using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using FluentAssertions;
using NUnit.Framework;

namespace FunctionalTests
{
    [TestFixture]
    public class TraceBinaryDeserializatorTests
    {
        private readonly TraceBinaryDeserializer deserializer = new TraceBinaryDeserializer();

        private static IEnumerable<TestCaseData> GetTests()
        {
            yield return new TestCaseData(new byte[] { 0b1111_1111 }, Trace.From(new HaltCommand()), true).SetName("Halt");

            yield return new TestCaseData(new byte[] { 0b1111_1110 }, Trace.From(new WaitCommand()), true).SetName("Wait");

            yield return new TestCaseData(new byte[] { 0b1111_1101 }, Trace.From(new FlipCommand()), true).SetName("Flip");

            yield return new TestCaseData(
                new byte[] { 0b0001_0100, 0b0001_1011 },
                Trace.From(new SMoveCommand(new Vector(12, 0, 0))),
                false)
                .SetName("SMove");

            yield return new TestCaseData(
                new byte[] { 0b0011_0100, 0b0000_1011 },
                Trace.From(new SMoveCommand(new Vector(0, 0, -4))),
                    false)
                .SetName("SMove with negative");

            yield return new TestCaseData(
                new byte[] { 0b1001_1100, 0b0000_1000 },
                Trace.From(new LMoveCommand(new Vector(3, 0, 0), new Vector(0, -5, 0))),
                    false)
                .SetName("LMove 1");

            yield return new TestCaseData(
                    new byte[] { 0b1110_1100, 0b0111_0011 },
                    Trace.From(new LMoveCommand(new Vector(0, -2, 0), new Vector(0, 0, 2))),
                    false)
                .SetName("LMove 2");

            yield return new TestCaseData(
                    new byte[] { 0b0011_1111 },
                    Trace.From(new FusionPCommand(new Vector(-1, 1, 0))),
                    false)
                .SetName("FusionP");

            yield return new TestCaseData(
                    new byte[] { 0b1001_1110 },
                    Trace.From(new FusionSCommand(new Vector(1, -1, 0))),
                    false)
                .SetName("FusionS");

            yield return new TestCaseData(
                    new byte[] { 0b0111_0101, 0b0000_0101 },
                    Trace.From(new FissionCommand(new Vector(0, 0, 1), 5)),
                    false)
                .SetName("Fission");

            yield return new TestCaseData(
                    new byte[] { 0b0101_0011 },
                    Trace.From(new FillCommand(new Vector(0, -1, 0))),
                    false)
                .SetName("Fill");
        }

        [TestCaseSource(nameof(GetTests))]
        public void Deserialize_ShouldReturnExpectedTrace(byte[] bytes, Trace expectedTrace, bool isSimpleType)
        {
            var sut = deserializer.Deserialize(bytes);
            if (isSimpleType)
            {
                sut.Count.Should().Be(expectedTrace.Count);
                sut.Select(x => x.GetType())
                    .Zip(expectedTrace.Select(e => e.GetType()), (type, type1) => type == type1)
                    .All(x => x).Should().BeTrue();
            }
            else
            {
                sut.Should().BeEquivalentTo(expectedTrace, op => op.WithStrictOrdering().RespectingRuntimeTypes());
            }
            
        }
    }
}
