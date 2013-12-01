using System;
using System.Drawing;
using System.Collections.Generic;

namespace Polygen
{
	public class PolyPoint
	{
		public PolyPoint()
		{
			X = 0;
			Y = 0;
		}

		public PolyPoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }

		public static implicit operator Point(PolyPoint p)
		{
			return new Point(p.X, p.Y);
		}

		public static implicit operator PolyPoint(System.Drawing.Point p)
		{
			return new PolyPoint(p.X, p.Y);
		}

		public PolyPoint Abs()
		{
			return new PolyPoint(Math.Abs(X), Math.Abs(Y));
		}

		public static PolyPoint operator -(PolyPoint p1, PolyPoint p2)
		{
			return new PolyPoint(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static PolyPoint operator +(PolyPoint p1, PolyPoint p2)
		{
			return new PolyPoint(p1.X + p2.X, p1.Y + p2.Y);
		}

		public override bool Equals(object obj)
		{
			var pp = obj as PolyPoint;

			if (pp != null)
				return (pp.X == X) && (pp.Y == Y);

			// Not sure if this case is needed due to the implicit conversion operator
			if (obj is Point)
			{
				var p = (Point)obj;
				return (p.X == X) && (p.Y == Y);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public static double DistanceBetween(PolyPoint p1, PolyPoint p2)
		{
			PolyPoint diff = (p1 - p2).Abs();

			return Math.Sqrt(
				(diff.X * diff.X) +
				(diff.Y * diff.Y));
		}

		public double DistanceFrom(PolyPoint p)
		{
			return PolyPoint.DistanceBetween(this, p);
		}
	}
}