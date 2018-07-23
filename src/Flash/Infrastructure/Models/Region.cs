using System;
using System.Collections.Generic;

namespace Flash.Infrastructure.Models
{
	public class Region
	{
		public readonly Vector Max;
		public readonly Vector Min;

		public Region(Vector point) :
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

		public (int Begin, int End) ProjectionX => (Min.X, Min.X + (Max - Min).X);
		public (int Begin, int End) ProjectionY => (Min.Y, Min.Y + (Max - Min).Y);
		public (int Begin, int End) ProjectionZ => (Min.Z, Min.Z + (Max - Min).Z);

		public bool AreIntersectsWith(Region region)
		{
			return SegmentsIntersects(ProjectionX, region.ProjectionX)
				   && SegmentsIntersects(ProjectionY, region.ProjectionY)
				   && SegmentsIntersects(ProjectionZ, region.ProjectionZ);
		}

		private static bool SegmentsIntersects((int Begin, int End) first, (int Begin, int End) second)
		{
			return Math.Max(first.Begin, second.Begin) <= Math.Min(first.End, second.End);
		}

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
			return Equals((Region)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Max != null ? Max.GetHashCode() : 0) * 397) ^ (Min != null ? Min.GetHashCode() : 0);
			}
		}

		#endregion

		public int Volume => (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);

		public override string ToString() => $"[{Min}, {Max}]";

		public List<Vector> Points()
		{
			if (Dim == 0)
			{
				return new List<Vector> { Min };
			}
			if (Dim == 1)
			{
				return new List<Vector> { Min, Max };
			}
			if (Dim == 2)
			{
				var a = Max - Min;
				if (a.X == 0)
				{
					return new List<Vector> { Min, Min + new Vector(0, a.Y, 0), Min + new Vector(0, 0, a.Z), Max };
				}
				if (a.Y == 0)
				{
					return new List<Vector> { Min, Min + new Vector(a.X, 0, 0), Min + new Vector(0, 0, a.Z), Max };
				}
				else
				{
					return new List<Vector> { Min, Min + new Vector(a.X, 0, 0), Min + new Vector(0, a.Y, 0), Max };
				}

			}
			else
			{
				var a = Max - Min;
				var p1 = Min + new Vector(a.X, 0, 0);
				var p2 = Min + new Vector(0, a.Y, 0);
				var p3 = Min + new Vector(0, 0, a.Z);
				var p4 = Max - new Vector(a.X, 0, 0);
				var p5 = Max - new Vector(0, a.Y, 0);
				var p6 = Max - new Vector(0, 0, a.Z);

				return new List<Vector> { p1, p2, p3, p4, p5, p6, Min, Max };
			}
		}

		public List<Vector> RevPoints()
		{
			if (Dim == 0)
			{
				return new List<Vector> { Min };
			}
			if (Dim == 1)
			{
				return new List<Vector> { Max, Min };
			}
			if (Dim == 2)
			{
				var a = Max - Min;
				if (a.X == 0)
				{
					return new List<Vector> { Max, Min + new Vector(0, 0, a.Z), Min + new Vector(0, a.Y, 0), Min };
				}
				if (a.Y == 0)
				{
					return new List<Vector> { Max, Min + new Vector(0, 0, a.Z), Min + new Vector(a.X, 0, 0), Min };
				}
				else
				{
					return new List<Vector> { Max, Min + new Vector(0, a.Y, 0), Min + new Vector(a.X, 0, 0), Min };
				}

			}
			else
			{
				var a = Max - Min;
				var p1 = Min + new Vector(a.X, 0, 0);
				var p2 = Min + new Vector(0, a.Y, 0);
				var p3 = Min + new Vector(0, 0, a.Z);
				var p4 = Max - new Vector(a.X, 0, 0);
				var p5 = Max - new Vector(0, a.Y, 0);
				var p6 = Max - new Vector(0, 0, a.Z);

				return new List<Vector> { p4, p5, p6, p1, p2, p3, Max, Min };
			}
		}
	}
}
