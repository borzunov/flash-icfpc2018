using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Flash.Infrastructure.Tools;

namespace Flash.Infrastructure.Deserializers
{
    public static class MatrixDeserializer
    {
        public static Matrix Deserialize(byte[] bytes)
        {
            var r = bytes[0];
            var matrix = new Matrix(r);
           
            var bitReader = new BitReader(bytes.Skip(1).ToArray());
            for (var x = 0; x < r; x++)
            {
                for (var y = 0; y < r; y++)
                {
                    for (var z = 0; z < r; z++)
                    {
                        var bit = bitReader.GetBit(x * r * r + y * r + z);
                        if (bit == 1)
                            matrix.Fill(new Vector(x, y, z));
                    }
                }
            }
            return matrix;
        }
    }
}