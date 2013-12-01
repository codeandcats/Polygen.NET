using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Drawing;

namespace Polygen.Schema
{
	[Serializable]
	public struct ColoredPoint
	{
		public ColoredPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.color = System.Drawing.Color.Black;
		}

		public ColoredPoint(int x, int y, SerializableColor color)
		{
			this.x = x;
			this.y = y;
			this.color = color;
		}

		public ColoredPoint(Point point)
		{
			this.x = point.X;
			this.y = point.Y;
			this.color = (SerializableColor)System.Drawing.Color.Black;
		}

		private int x;
		public int X { get { return x; } set { x = value; } }

		private int y;
		public int Y { get { return y;} set { y = value; } }

		public Point Point
		{
			get
			{
				return new Point(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		private SerializableColor color;
		public SerializableColor Color { get { return color; } set { color = value; } }

		public override string ToString()
		{
			return string.Format("{0},{1} {2}", X, Y, Color.ToString());
		}

		public static implicit operator Point(ColoredPoint coloredPoint)
		{
			return coloredPoint.Point;
		}

		public static implicit operator ColoredPointF(ColoredPoint coloredPoint)
		{
			return new ColoredPointF(coloredPoint.X, coloredPoint.Y, coloredPoint.Color);
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
			int hashCode = X ^ Y ^ Color.GetHashCode();
			return hashCode;
		}
	}
}
