using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    public class FusionSCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;
            var codedNd = (byte)((bytes[offset] & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new FusionSCommand(nearDistance);
        }
    }
}