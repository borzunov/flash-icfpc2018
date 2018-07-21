namespace Flash.Infrastructure.Tools
{
    public  class BitReader
    {
        private readonly byte[] bytes;

        public  BitReader(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public int GetBit(int index)
        {
            var byteP = index / 8;
            var bitP = 7 - index % 8;

            return (bytes[byteP] >> bitP) & 1;
        }
    }
}