using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class HaltCommandSerializer : BaseCommandSerializer<HaltCommand>
    {
        protected override byte[] Serialize(HaltCommand command)
        {
            return new byte[] {0b1111_1111};
        }
    }
}