using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Polygen.Schema;
using System.Drawing.Drawing2D;

namespace Polygen
{
	public static class GeometeryUtils
	{
		public static bool PolygonContainsPoint(Point[] polygonPoints, Point point)
		{
			if (polygonPoints.Length < 3)
				return false;

			// Do a simple search on a bounding box first
			int minX = polygonPoints[0].X;
			int maxX = minX;
			int minY = polygonPoints[0].Y;
			int maxY = minY;

			foreach (var loopPoint in polygonPoints)
			{
				minX = Math.Min(loopPoint.X, minX);
				maxX = Math.Max(loopPoint.X, maxX);
				minY = Math.Min(loopPoint.Y, minY);
				maxY = Math.Max(loopPoint.Y, maxY);
			}

			if ((point.X >= minX) && (point.X < maxX) && (point.Y >= minY) && (point.Y <= maxY))
			{
				// Okay, the point is within the bounding box so let's do a more comprehensive test
				var path = new GraphicsPath();
				path.AddLines(polygonPoints);
				return path.IsVisible(point);
			}
			else
				return false;
		}

		public static bool FindLineIntersection(
			PointF start1, PointF end1, 
			PointF start2, PointF end2, 
			out PointF intersection)
		{
			intersection = PointF.Empty;

			float denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));

			//  AB & CD are parallel 
			if (denom == 0)
				return false;

			float numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));

			float r = numer / denom;

			float numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));

			float s = numer2 / denom;

			if ((r < 0 || r > 1) || (s < 0 || s > 1))
				return false;

			// Find intersection point
			intersection = new PointF(
				start1.X + (r * (end1.X - start1.X)),
				start1.Y + (r * (end1.Y - start1.Y)));

			return true;
		}

		// Only works with convex polygons in CW/CCW order
		public static bool IsLineIntersectingPolygon(
			Point lineStart,
			Point lineEnd,
			Point[] polygonPoints,
			bool ignorePolygonEdges = false)
		{
			int pointCount = polygonPoints.Count();

			Point[] polygonEdgePoints = null;

			for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
			{
				Point p1 = polygonPoints.ElementAt(pointIndex);
				Point p2 = polygonPoints.ElementAt((pointIndex + 1) % pointCount);

				if (ignorePolygonEdges && 
					(p1.Equals(lineStart) || p1.Equals(lineEnd) || p2.Equals(lineStart) || p2.Equals(lineEnd)))
				{
					continue;
				}

				PointF intersection;
				if (FindLineIntersection(lineStart, lineEnd, p1, p2, out intersection))
				{
					///*
					if (ignorePolygonEdges)
					{
						// Calculate the edge points of this polygon
						if (polygonEdgePoints == null)
							polygonEdgePoints = GetPointsInPolygonEdges(polygonPoints.ToArray()).ToArray();

						// Check to see if any points in the line intersect the polygon 
						// anywhere other than it's edges
						foreach (var linePoint in GetPointsInLine(lineStart, lineEnd))
						{
							if (polygonEdgePoints.Contains(linePoint))
								continue;

							if (PolygonContainsPoint(polygonPoints, linePoint))
								return true;
						}
					}
					else
						return true;
					//*/
					return true;
				}
			}

			return false;
		}

		public static IEnumerable<Point> GetPointsInPolygonEdges(Point[] polygon)
		{
			Point prevPoint = default(Point);
			bool isFirstPoint = true;

			for (var pointIndex = 0; pointIndex < polygon.Length; pointIndex++)
			{
				Point p1 = polygon[pointIndex];
				Point p2 = polygon[(pointIndex + 1) % polygon.Length];

				foreach (var point in GetPointsInLine(p1, p2))
				{
					if (isFirstPoint || (!point.Equals(prevPoint)))
					{
						yield return point;
						prevPoint = point;
						isFirstPoint = false;
					}
				}
			}
		}

		public static IEnumerable<Point> GetPointsInLine(Point lineStart, Point lineEnd)
		{
			if (lineStart.Equals(lineEnd))
			{
				yield return lineStart;
				yield break;
			}

			int xDiff = lineEnd.X - lineStart.X;
			int yDiff = lineEnd.Y - lineStart.Y;
			int absoluteXDiff = Math.Abs(lineStart.X - lineEnd.X);
			int absoluteYDiff = Math.Abs(lineStart.Y - lineEnd.Y);

			int lineLength = (int)Math.Sqrt(
				(Math.Pow(absoluteXDiff, 2) +
				Math.Pow(absoluteYDiff, 2)));

			int steps = lineLength;
			double xStep = xDiff / (double)steps;
			double yStep = yDiff / (double)steps;

			Point[] result = new Point[steps];

			for (int step = 0; step <= steps; step++)
			{
				int x = (int)Math.Round(lineStart.X + (xStep * step));
				int y = (int)Math.Round(lineStart.Y + (yStep * step));
				yield return new Point(x, y);
			}
		}

		public static IEnumerable<Point> GetAllPoints(this IEnumerable<Polygon> polygons)
		{
			foreach (var polygon in polygons)
			{
				foreach (var point in polygon.Points)
				{
					yield return point;
				}
			}
		}

		public static double DistanceBetween(Point point1, Point point2)
		{
			int diffX = Math.Abs(point1.X - point2.X);
			int diffY = Math.Abs(point1.Y - point2.Y);

			return Math.Sqrt((diffX * diffX) + (diffY * diffY));
		}

		public static double DistanceBetween(PointF point1, PointF point2)
		{
			float diffX = Math.Abs(point1.X - point2.X);
			float diffY = Math.Abs(point1.Y - point2.Y);

			return Math.Sqrt((diffX * diffX) + (diffY * diffY));
		}

		public static double DistanceFrom(this Point thisPoint, Point point)
		{
			return DistanceBetween(thisPoint, point);
		}

		public static double DistanceFrom(this Point thisPoint, PointF point)
		{
			return DistanceBetween(thisPoint, point);
		}

		public static PointF GetCenter(this Rectangle rect)
		{
			return new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
		}

		public static Point Round(this PointF point)
		{
			return new Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
		}

		public static Point ClipTo(this Point point, Rectangle rectangle)
		{
			return new Point(
				Math.Max(Math.Min(point.X, rectangle.Right), rectangle.Left),
				Math.Max(Math.Min(point.Y, rectangle.Bottom), rectangle.Top));
		}

		public static PointF Subtract(this Point thisPoint, PointF subtract)
		{
			return new PointF(
				thisPoint.X - subtract.X,
				thisPoint.Y - subtract.Y);
		}

		public static PointF Add(this Point thisPoint, PointF add)
		{
			return new PointF(
				thisPoint.X + add.X,
				thisPoint.Y + add.Y);
		}

		public static PointF Subtract(this PointF thisPoint, PointF subtract)
		{
			return new PointF(
				thisPoint.X - subtract.X,
				thisPoint.Y - subtract.Y);
		}

		public static PointF Add(this PointF thisPoint, PointF add)
		{
			return new PointF(
				thisPoint.X + add.X,
				thisPoint.Y + add.Y);
		}

		public static Point GetBottomRight(this Rectangle thisRect)
		{
			return new Point(thisRect.Right, thisRect.Bottom);
		}

		public static double GetPolygonArea(Point[] polygon)
		{
			int i, j;
			double area = 0;

			for (i = 0; i < polygon.Length; i++)
			{
				j = (i + 1) % polygon.Length;

				area += (double)polygon[i].X * (double)polygon[j].Y;
				area -= (double)polygon[i].Y * (double)polygon[j].X;
			}

			area /= 2;
			return (area < 0 ? -area : area);
		}

		public static IEnumerable<Point> GetAllPointsInPolygonUsingGraphicsPath(Point[] cornerPoints, Bitmap image)
		{
			using (Graphics graphics = Graphics.FromImage(image))
			{
				// Get the min and max bounds for this polygon
				Point min = new Point(int.MaxValue, int.MaxValue);
				Point max = new Point(int.MinValue, int.MinValue);

				foreach (var point in cornerPoints)
				{
					if (point.X < min.X)
						min.X = point.X;

					if (point.X > max.X)
						max.X = point.X;

					if (point.Y < min.Y)
						min.Y = point.Y;

					if (point.Y > max.Y)
						max.Y = point.Y;
				}

				var bitmapBounds = new Rectangle(min.X, min.Y, max.X, max.Y);

				var path = new System.Drawing.Drawing2D.GraphicsPath();
				path.AddPolygon(cornerPoints.ToArray());
				path.CloseAllFigures();

				var region = new System.Drawing.Region(path);

				for (var y = min.Y; y <= max.Y; y++)
				{
					for (var x = min.X; x <= max.X; x++)
					{
						if (region.IsVisible(x, y, graphics) && bitmapBounds.Contains(x, y))
						{
							yield return new Point(x, y);
						}
					}
				}
			}
		}

		public static IEnumerable<Point> GetAllPointsInPolygonUsingRayTrace(Point[] cornerPoints)
		{
			if (cornerPoints.Length == 0)
				yield break;

			var min = cornerPoints[0];
			var max = cornerPoints[0];

			foreach (var loopPoint in cornerPoints)
			{
				if (min.X > loopPoint.X)
					min.X = loopPoint.X;

				if (max.X < loopPoint.X)
					max.X = loopPoint.X;

				if (min.Y > loopPoint.Y)
					min.Y = loopPoint.Y;

				if (max.Y < loopPoint.Y)
					max.Y = loopPoint.Y;
			}

			var polygonBounds = new Rectangle(min.X, min.Y, max.X - min.X, max.Y - min.Y);

			Point[] edgePoints = GetPointsInPolygonEdges(cornerPoints).ToArray();

			var polyBounds = new Rectangle(min.X, min.Y, max.X, max.Y);
			polyBounds.Inflate(1, 1);

			Point point = Point.Empty;
			for (var y = min.Y; y <= max.Y; y++)
			{
				for (var x = min.X; x <= max.X; x++)
				{
					point.X = x;
					point.Y = y;

					if (IsPointInPolygonUsingRayTrace(ref edgePoints, polygonBounds, point))
					{
						yield return point;
					}
				}
			}
		}

		public static bool IsPointInPolygonUsingRayTrace(ref Point[] polygonCorners, Point point)
		{
			Point[] edgePoints = GetPointsInPolygonEdges(polygonCorners).ToArray();

			if (edgePoints.Length == 0)
				return false;

			//var polygonBounds = new Rectangle(edgePoints[0].X, edgePoints[0].Y, 0, 0);
			var min = edgePoints[0];
			var max = edgePoints[0];

			for (var index = 0; index < edgePoints.Length; index++)
			{
				var loopPoint = edgePoints[index];
				if (min.X > loopPoint.X)
					min.X = loopPoint.X;

				if (max.X < loopPoint.X)
					max.X = loopPoint.X;

				if (min.Y > loopPoint.Y)
					min.Y = loopPoint.Y;

				if (max.Y < loopPoint.Y)
					max.Y = loopPoint.Y;
			}

			var polygonBounds = new Rectangle(min.X, min.Y, max.X - min.X, max.Y - min.Y);
			//polygonBounds.Inflate(1, 1);

			return IsPointInPolygonUsingRayTrace(
				ref edgePoints,
				polygonBounds,
				point);
		}

		/*
		public static bool IsPointInPolygonUsingRayTrace(
			ref Point[] polygonEdges,
			Rectangle polygonBounds,
			Point point)
		{
			if (!polygonBounds.Contains(point))
				return false;

			int topDiff = point.Y - polygonBounds.Top;
			int bottomDiff = polygonBounds.Bottom - point.Y;
			int leftDiff = point.X - polygonBounds.Left;
			int rightDiff = polygonBounds.Right - point.X;

			int directionX;
			int directionY;

			if (topDiff < bottomDiff)
				directionY = -(topDiff + 1);
			else
				directionY = bottomDiff + 1;

			if (leftDiff < rightDiff)
				directionX = -(leftDiff + 1);
			else
				directionX = rightDiff + 1;

			if (Math.Abs(directionX) < Math.Abs(directionY))
				directionY = 0;
			else
				directionX = 0;

			directionX = directionX.CompareTo(0);
			directionY = directionY.CompareTo(0);

			Point walkPoint = point;
			int edgeCount = 0;
			do
			{
				if (polygonEdges.Contains(walkPoint))
					edgeCount++;

				walkPoint.X = walkPoint.X + directionX;
				walkPoint.Y = walkPoint.Y + directionY;
			}
			while (polygonBounds.Contains(walkPoint));

			return (edgeCount % 2) == 1;
		}
		//*/

		///*
		public static bool IsPointInPolygonUsingRayTrace(
			ref Point[] polygonEdges,
			Rectangle polygonBounds,
			Point point)
		{
			if (!polygonBounds.Contains(point))
				return false;

			int topDiff = point.Y - polygonBounds.Top;
			int bottomDiff = polygonBounds.Bottom - point.Y;
			int leftDiff = point.X - polygonBounds.Left;
			int rightDiff = polygonBounds.Right - point.X;

			int directionX;
			int directionY;

			if (topDiff < bottomDiff)
				directionY = -(topDiff + 1);
			else
				directionY = bottomDiff + 1;

			if (leftDiff < rightDiff)
				directionX = -(leftDiff + 1);
			else
				directionX = rightDiff + 1;

			if (Math.Abs(directionX) < Math.Abs(directionY))
				directionY = 0;
			else
				directionX = 0;

			int edgeCount = 0;

			if (directionX < 0)
			{
				foreach (Point edgePoint in polygonEdges)
					if ((edgePoint.Y == point.Y) && (edgePoint.X <= point.X))
						edgeCount++;
			}
			else if (directionX > 0)
			{
				foreach (Point edgePoint in polygonEdges)
					if ((edgePoint.Y == point.Y) && (edgePoint.X >= point.X))
						edgeCount++;
			}
			else if (directionY < 0)
			{
				foreach (Point edgePoint in polygonEdges)
					if ((edgePoint.X == point.X) && (edgePoint.Y <= point.Y))
						edgeCount++;
			}
			else
			{
				foreach (Point edgePoint in polygonEdges)
					if ((edgePoint.X == point.X) && (edgePoint.Y >= point.Y))
						edgeCount++;
			}

			return (edgeCount % 2) == 1;
		}
		//*/

		public static PointF GetPolygonCenter(Point[] polygon)
		{
			PointF center = new PointF();
			center.X = polygon.Average(p => (float)p.X);
			center.Y = polygon.Average(p => (float)p.Y);
			return center;
		}

		public static PointF GetPolygonCenter(PointF[] polygon)
		{
			PointF center = new PointF();
			center.X = polygon.Average(p => p.X);
			center.Y = polygon.Average(p => p.Y);
			return center;
		}

		public static PointF GetPolygonCenter(ColoredPoint[] polygon)
		{
			PointF center = new PointF();
			center.X = polygon.Average(p => (float)p.X);
			center.Y = polygon.Average(p => (float)p.Y);
			return center;
		}

		public static Point[] GetConvexPolygonPointsSorted(Point[] points)
		{
			PointF center = GetPolygonCenter(points);
			var newOrder = points.OrderBy(p => p, new PolygonPointComparer(center)).ToArray();
			return newOrder;
		}

		public static ColoredPoint[] GetConvexPolygonPointsSorted(ColoredPoint[] points)
		{
			PointF center = GetPolygonCenter(points);
			var newOrder = points.OrderBy(p => p, new PolygonColoredPointComparer(center)).ToArray();
			return newOrder;
		}

		private class PolygonPointComparer : IComparer<Point>
		{
			public PolygonPointComparer(PointF centerPoint)
			{
				this.centerPoint = centerPoint;
			}

			private PointF centerPoint;
			
			public int Compare(Point a, Point b)
			{
				//  Variables to Store the atans
				double aTanA, aTanB;

				//  Fetch the atans
				aTanA = Math.Atan2(a.Y - centerPoint.Y, a.X - centerPoint.X);
				aTanB = Math.Atan2(b.Y - centerPoint.Y, b.X - centerPoint.X);

				//  Determine next point in Clockwise rotation
				return aTanA.CompareTo(aTanB);
			}
		}

		private class PolygonColoredPointComparer : IComparer<Point>
		{
			public PolygonColoredPointComparer(PointF centerPoint)
			{
				comparer = new PolygonPointComparer(centerPoint);
			}

			private PolygonPointComparer comparer;

			public int Compare(Point a, Point b)
			{
				return comparer.Compare(a, b);
			}
		}

		public static bool Contains(this IEnumerable<ColoredPoint> points, Point point)
		{
			foreach (var loopPoint in points)
				if (loopPoint.Equals(point))
					return true;

			return false;
		}
	}
}
