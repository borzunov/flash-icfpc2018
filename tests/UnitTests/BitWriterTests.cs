using System;
using Flash.Infrastructure.Serializers;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{

    [TestFixture]
    public class BitWriterTests
    {
        [Test]
        public void Write_AddsBitToResult()
        {
            var bytes = BitWriter.Start()
                .WriteZero(3)
                .WriteOne(3)
                .WriteZero()
                .WriteOne()
                .EndOfFirstByte()
                .ToBytes();

            bytes.Should().BeEquivalentTo(new byte[] {0b00011101});
        }

        [Test]
        public void EndOfFirstByte_NoEnoughtBitsToEnd_ShouldThrowInvalidOperationException()
        {
            Action sut = () => BitWriter.Start()
                .WriteZero(3)
                .WriteOne(3)
                .WriteZero()
                .EndOfFirstByte()
                .ToBytes();

            sut.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Write_TwoBytes_ShouldCreateExpectedBytes()
        {
            var bytes = BitWriter.Start()
                .WriteZero(7)
                .WriteOne()
                .EndOfFirstByte()
                .WriteOne(7)
                .WriteZero()
                .EndOfSecondByte()
                .ToBytes();

            bytes.Should().BeEquivalentTo(new byte[] { 0b00000001, 0b11111110 }, op => op.WithStrictOrdering());
        }
    }
}
