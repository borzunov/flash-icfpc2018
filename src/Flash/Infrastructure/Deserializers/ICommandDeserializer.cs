using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    public interface ICommandDeserializer
    {
        ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount);
    }


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


    class LMoveCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 2;

            return null;
//            return new LMoveCommand();
        }
    }


    class FusionSCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;
            var codedNd = (byte)((bytes[offset] & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new FusionSCommand(nearDistance);
        }
    }

    class FusionPCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;
            var codedNd = (byte)((bytes[offset] & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new FusionPCommand(nearDistance);
        }
    }

    class FissionCommandDeserializer : ICommandDeserializer
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

    class FillCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;
            var codedNd = (byte)((bytes[offset] & 0b1111_1000) >> 3);
            var nearDistance = VectorDeserializer.DeserializeNearDifference(codedNd);

            return new FillCommand(nearDistance);
        }
    }

    class FlipCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;

            return new FlipCommand();
        }
    }

    class WaitCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;

            return new WaitCommand();
        }
    }

    class HaltCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;

            return new HaltCommand();
        }
    }
}