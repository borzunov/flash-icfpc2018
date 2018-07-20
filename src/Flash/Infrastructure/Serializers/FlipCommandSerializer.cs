using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FlipCommandSerializer : BaseCommandSerializer<FlipCommand>
    {
        protected override byte[] Serialize(FlipCommand command)
        {
            return new byte[] {0b1111_1101};
        }
    }
}