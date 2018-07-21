namespace Flash.Infrastructure.Serializers
{
    public static class BitSetExtensions
    {
        public static BitWriter WriteLcdAxis(this BitWriter bs, Vector vector)
        {
            VectorSerializer.SerializeLcdAxis(vector, bs);

            return bs;
        }

        public static BitWriter WriteLinearShortLength(this BitWriter bs, Vector vector)
        {
            VectorSerializer.SerializeLinearShortLength(vector, bs);

            return bs;
        }

        public static BitWriter WriteLinearLongLength(this BitWriter bs, Vector vector)
        {
            VectorSerializer.SerializeLinearLongLength(vector, bs);

            return bs;
        }

        public static BitWriter WriteNearDifference(this BitWriter bs, Vector vector)
        {
            VectorSerializer.SerializeNearDifference(vector, bs);

            return bs;
        }
    }
}