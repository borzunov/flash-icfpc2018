using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Serializers;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class CommandsSerializersTests
    {
        private static IEnumerable<TestCaseData> GetCommandsSerializerTests()
        {
            yield return new TestCaseData(new HaltCommandSerializer(), new HaltCommand(), new byte[]{ 0b1111_1111 })
                .SetName("Halt serializer test");
        }

        [TestCaseSource(nameof(GetCommandsSerializerTests))]
        public void Serialize_ShouldReturnExpectedBytes(
            ICommandSerializer serializer,
            ICommand command,
            byte[] expectedBytes)
        {
            var ms = new MemoryStream();

            serializer.Serialize(command, ms);

            var sut = ms.ToArray();
            sut.Should().BeEquivalentTo(expectedBytes, op => op.WithStrictOrdering());
        }
    }
}
