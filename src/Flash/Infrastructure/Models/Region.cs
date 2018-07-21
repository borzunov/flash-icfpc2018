using System;

namespace Flash.Infrastructure.Models
{
    public class Region
    {
        public readonly Vector Max;
        public readonly Vector Min;

        public Region(Vector point):
            this(point, point)
        {
        }

        public Region(Vector from, Vector to)
        {
            Min = new Vector(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y), Math.Min(from.Z, to.Z));
            Max = new Vector(Math.Max(from.X, to.X), Math.Max(from.Y, to.Y), Math.Max(from.Z, to.Z));
        }

        public bool Contains(Vector v)
        {
            return Min.X <= v.X && v.X <= Max.X &&
                   Min.Y <= v.Y && v.Y <= Max.Y &&
                   Min.Z <= v.Z && v.Z <= Max.Z;
        }

        public int Dim => (Min.X == Max.X ? 0 : 1) +
                          (Min.Y == Max.Y ? 0 : 1) +
                          (Min.Z == Max.Z ? 0 : 1);

        #region equality members
        
        protected bool Equals(Region other)
        {
            return Equals(Max, other.Max) && Equals(Min, other.Min);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Region) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Max != null ? Max.GetHashCode() : 0)*397) ^ (Min != null ? Min.GetHashCode() : 0);
            }
        }

        #endregion
    }
}
