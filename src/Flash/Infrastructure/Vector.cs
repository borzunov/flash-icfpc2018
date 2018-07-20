using System;

namespace Flash.Infrastructure
{
    public class Vector
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Vector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Manhattan length
        /// </summary>
        public int Mlen => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        /// <summary>
        /// Chessboard length
        /// </summary>
        public int Clen => Math.Max(Math.Max(Math.Abs(X), Math.Abs(Y)), Math.Abs(Z));

        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        public static Vector operator -(Vector v) => new Vector(-v.X, -v.Y, -v.Z);

        /// <summary>
        /// (this - other).Mlen == 1;
        /// </summary>
        public bool IsAdjacent(Vector other)
        {
            return (this - other).Mlen == 1;
        }

        #region equality members
        protected bool Equals(Vector other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Vector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode*397) ^ Y;
                hashCode = (hashCode*397) ^ Z;
                return hashCode;
            }
        }
        #endregion
    }
}
