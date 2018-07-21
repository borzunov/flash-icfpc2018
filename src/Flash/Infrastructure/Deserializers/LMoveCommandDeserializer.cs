using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    class LMoveCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 2;
            var firstByte = bytes[offset];
            var secondByte = bytes[offset + 1];
            var firstUnitVector = VectorDeserializer.DeserializeLcdAxis((byte)((firstByte & 0b0011_0000) >> 4));
            var secondUnitVector = VectorDeserializer.DeserializeLcdAxis((byte)((firstByte & 0b1100_0000) >> 6));
            var firstLength = VectorDeserializer.DeserializeLinearShortLength((byte)(secondByte & 0b0000_1111));
            var secondLength = VectorDeserializer.DeserializeLinearShortLength((byte)((secondByte & 0b1111_0000) >> 4));
            var firstDirection = firstUnitVector * firstLength;
            var secondDirection = secondUnitVector * secondLength;

            return new LMoveCommand(firstDirection, secondDirection);
        }
    }
}