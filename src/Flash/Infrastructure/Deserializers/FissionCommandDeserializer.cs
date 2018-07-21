﻿using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    public class FissionCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 2;
            var firstByte = bytes[offset];
            var secondByte = bytes[offset + 1];
            var codedNd = (byte)((firstByte & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new FissionCommand(nearDistance, secondByte);
        }
    }
}