using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Drawing;

namespace Polygen.Schema
{
	[Serializable]
	public struct ColoredPointF
	{
		public ColoredPointF(int x, int y, SerializableColor color)
		{
			this.x = x;
			this.y = y;
			this.color = color;
		}

		private float x;
		public float X { get { return x; } set { x = value; } }

		private float y;
		public float Y { get { return y; } set { y = value; } }

		public PointF Point
		{
			get
			{
				return new PointF(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public ColoredPoint Round()
		{
			return new ColoredPoint((int)Math.Round(X), (int)Math.Round(Y), Color);
		}

		private SerializableColor color;
		public SerializableColor Color { get { return color; } set { color = value; } }

		public override string ToString()
		{
			return string.Format("{0},{1} {2}", X, Y, Color.ToString());
		}

		public static implicit operator PointF(ColoredPointF coloredPoint)
		{
			return coloredPoint.Point;
		}

		public static explicit operator ColoredPoint(ColoredPointF coloredPoint)
		{
			return new ColoredPoint(
				(int)Math.Round(coloredPoint.X),
				(int)Math.Round(coloredPoint.Y),
				coloredPoint.Color);
		}

		public override bool Equals(object obj)
		{
			if (obj is Point)
			{
				var point = (Point)obj;

				return ((X == point.X) && (Y == point.Y));
			}

			if (obj is PointF)
			{
				var pointF = (PointF)obj;

				return ((pointF.X == (float)X) && (pointF.Y == (float)Y));
			}

			if (obj is ColoredPoint)
			{
				var coloredPoint = (ColoredPoint)obj;

				return ((coloredPoint.X == X) && (coloredPoint.Y == Y) && coloredPoint.Color.Equals(Color));
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hashCode = X.GetHashCode() ^ Y.GetHashCode() ^ Color.GetHashCode();
			return hashCode;
		}
	}
}
