using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class WaitCommandSerializer : BaseCommandSerializer<WaitCommand>
    {
        protected override byte[] Serialize(WaitCommand command)
        {
            return new byte[] {0b1111_1110};
        }
    }
}