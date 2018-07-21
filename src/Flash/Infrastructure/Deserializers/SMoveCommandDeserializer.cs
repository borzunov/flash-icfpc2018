using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    class SMoveCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 2;
            var firstByte = bytes[offset];
            var secondByte = bytes[offset + 1];
            var unitVector = VectorDeserializer.DeserializeLcdAxis((byte) ((firstByte << 2) >> 6));
            var length = VectorDeserializer.DeserializeLinearLongLength(secondByte);
            var direction = unitVector * length;

            return new SMoveCommand(direction);
        }
    }
}