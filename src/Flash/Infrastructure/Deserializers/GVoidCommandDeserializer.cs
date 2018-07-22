using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Deserializers
{
    class GVoidCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 4;
            var firstByte = bytes[offset];
            var codedNd = (byte)((firstByte & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new GVoidCommand(nearDistance,
                new Vector(bytes[offset + 1] - 30, bytes[offset + 2] - 30, bytes[offset + 3] - 30));
        }
    }
}