using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Deserializers
{
    public class FlipCommandDeserializer : ICommandDeserializer
    {
        public ICommand Desrialize(byte[] bytes, int offset, out int readBytesCount)
        {
            readBytesCount = 1;

            return new FlipCommand();
        }
    }
}