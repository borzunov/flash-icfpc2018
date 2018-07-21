using System;

namespace Flash.Infrastructure.Models
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
        public bool IsAdjacentTo(Vector other)
        {
            return (this - other).Mlen == 1;
        }

        public Vector[] GetAdjacents()
        {
            return new[]
            {
				//                new Vector(X, Y-1, Z), // ORDER is very important for check grounded
				//                new Vector(X+1, Y, Z),
				//                new Vector(X, Y, Z+1),
				//                new Vector(X-1, Y, Z),
				//                new Vector(X, Y, Z-1),
				//                new Vector(X, Y+1, Z)

	            new Vector(X, Y+1, Z), // ORDER is very important for check grounded
	            new Vector(X, Y, Z-1),
	            new Vector(X-1, Y, Z),
	            new Vector(X, Y, Z+1),
	            new Vector(X+1, Y, Z),
	            new Vector(X, Y-1, Z)

			};
        }

	    public Vector[] GetAdjacents(int n)
	    {
		    return new[]
		    {
			    new Vector(X, Y-n, Z), // ORDER is very important for check grounded
			    new Vector(X+n, Y, Z),
			    new Vector(X, Y, Z+n),
			    new Vector(X-n, Y, Z),
			    new Vector(X, Y, Z-n),
			    new Vector(X, Y+n, Z)
		    };
	    }

	    public Vector GetAdjacent(int s, int n)
	    {
		    return GetAdjacents(n)[s];
	    }

		/// <summary>
		/// is linear difference
		/// </summary>
		public bool IsLd => Mlen == Clen && Mlen != 0;

        /// <summary>
        /// is short linear difference
        /// </summary>
        public bool IsSld => IsLd && Mlen <= 5;

        /// <summary>
        /// is long linear difference
        /// </summary>
        public bool IsLld => IsLd && Mlen <= 15;

        /// <summary>
        /// is near difference
        /// </summary>
        public bool IsNd => Mlen > 0 && Mlen <= 2 && Clen == 1;

        public Vector Normalize()
        {
            return new Vector(X / Mlen, Y / Mlen, Z / Mlen);
        }

        public static Vector operator *(Vector v, int scalar) => new Vector(v.X * scalar, v.Y * scalar, v.Z * scalar);
        public static Vector operator *(int scalar, Vector v) => new Vector(v.X * scalar, v.Y * scalar, v.Z * scalar);

        public Vector[] GetNears()
        {
            return new[]
            {
                new Vector(X+1, Y, Z),
                new Vector(X, Y+1, Z),
                new Vector(X, Y, Z+1),
                new Vector(X-1, Y, Z),
                new Vector(X, Y-1, Z),
                new Vector(X, Y, Z-1),
                new Vector(X+1, Y+1, Z),
                new Vector(X, Y+1, Z+1),
                new Vector(X+1, Y, Z+1),
                new Vector(X-1, Y-1, Z),
                new Vector(X, Y-1, Z-1),
                new Vector(X-1, Y, Z-1),
                new Vector(X+1, Y-1, Z),
                new Vector(X, Y+1, Z-1),
                new Vector(X+1, Y, Z-1),
                new Vector(X-1, Y+1, Z),
                new Vector(X, Y-1, Z+1),
                new Vector(X-1, Y, Z+1)
            };
        }

        public int GetFirstNonZeroComponent()
        {
            if (X != 0)
                return X;

            if (Y != 0)
                return Y;

            if (Z != 0)
                return Z;

            throw new InvalidOperationException("There is no non zero component");
        }

	    public bool IsGood(int r)
	    {
		    return !(X < 0 || Y < 0 || Z < 0 || X >= r || Y >= r || Z >= r);
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

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}
