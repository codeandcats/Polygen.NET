using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Polygen;
using Polygen.Schema;
using Polygen.UI;
using System.Windows.Forms;

namespace Polygen.Tools
{
	public class PolygonTool : Tool
	{
		public PolygonTool(ImageCanvas canvas)
			: base(canvas)
		{
		}

		private enum PolygonToolMode
		{
			None,
			Add,
			Resize,
			Split
		}

		private PolygonToolMode mode;
		private PointF mouseDownImagePoint = PointF.Empty;
		private Polygon mouseDownPolygon = null;

		private List<Point> firstPolygonClientPoints = new List<Point>();

		protected internal override void DrawPolygon(Graphics graphics, Polygon polygon)
		{
			Point imageHotSpot = canvas.GetImageHotSpotSnapped().Round();

			bool resizingThisPolygon = false;

			PointF[] points;

			switch (mode)
			{
				case PolygonToolMode.Resize:
					points = polygon.Points
						// If we clicked on one of the points, translate the point to the mouse point
						.Select(point =>
						{
							if (point.Equals(mouseDownImagePoint.Round()))
							{
								resizingThisPolygon = true;
								return imageHotSpot;
							}
							else
								return point;
						})
						// Translate back to client coordinates
						.Select(point => canvas.ImageToClient(point))
						.ToArray();

					if (points.Length < 3)
						break;

					if (resizingThisPolygon)
					{
						graphics.FillPolygon(canvas.validPolygonBrush, points);
						graphics.DrawPolygon(canvas.validPolygonEdgePen, points);
					}
					else
						canvas.DefaultDrawPolygon(graphics, polygon);
					
					break;

				case PolygonToolMode.Split:
					points = polygon.Points
						.Select(point => canvas.ImageToClient(point.Point))
						.ToArray();

					if (points.Length < 3)
						break;

					if (polygon != mouseDownPolygon)
					{
						canvas.DefaultDrawPolygon(graphics, polygon);
					}
					else
					{
						Brush fillBrush;
						Pen edgePen;
						if (mouseDownPolygon.ContainsPoint(imageHotSpot))
						{
							fillBrush = canvas.validPolygonBrush;
							edgePen = canvas.validPolygonEdgePen;
						}
						else
						{
							fillBrush = canvas.invalidPolygonBrush;
							edgePen = canvas.invalidPolygonEdgePen;
						}

						graphics.FillPolygon(fillBrush, points);
						graphics.DrawPolygon(edgePen, points);

						// Now draw the new split edges
						foreach (var point in points)
						{
							graphics.DrawLine(edgePen, point, ImageToClient(imageHotSpot));
						}
					}
					break;

				default:
					canvas.DefaultDrawPolygon(graphics, polygon);
					break;
			}
		}

		protected internal override void AfterDrawnPolygons(Graphics graphics)
		{
			var keys = Control.ModifierKeys;

			const int pointRadius = 3;

			// Get hot spot in image coordinates
			Point imageHotSpot = GetImageHotSpotSnapped().Round();

			switch (mode)
			{
				case PolygonToolMode.Resize:
					// We've already handled the drawing of resizing polygon(s)
					break;

				case PolygonToolMode.Add:
					if (!keys.HasFlag(Keys.Shift))
					{
						// Find the closest 2 points, from our existing polygons,
						// that do not intersect with polygons along the way
						//CalculateClosestPoints(false);
						if (closestAddPoints.Length == 2)
						{
							var newPolygonPoints = new Point[]
							{
								closestAddPoints[0],
								closestAddPoints[1],
								ImageToClient(imageHotSpot).Round()
							};

							Brush brush;
							Pen pen;

							if (canAddPolygon)
							{
								brush = canvas.validPolygonBrush;
								pen = canvas.validPolygonEdgePen;
							}
							else
							{
								brush = canvas.invalidPolygonBrush;
								pen = canvas.invalidPolygonEdgePen;
							}

							graphics.FillPolygon(brush, newPolygonPoints);
							graphics.DrawPolygon(pen, newPolygonPoints);
						}
					}
					break;

				case PolygonToolMode.None:
					if (firstPolygonClientPoints.Count > 0)
					{
						// Draw lines between points
						if (firstPolygonClientPoints.Count > 1)
						{
							for (int index = 1; index < firstPolygonClientPoints.Count; index++)
							{
								var point = firstPolygonClientPoints[index];
								var prevPoint = firstPolygonClientPoints[index - 1];

								graphics.DrawPolygon(canvas.validPolygonEdgePen, firstPolygonClientPoints.ToArray());
							}
						}

						// Draw dots for points
						for (int index = 0; index < firstPolygonClientPoints.Count; index++)
						{
							var clientPoint = firstPolygonClientPoints[index];

							graphics.FillCircle(canvas.validPolygonBrush, clientPoint, pointRadius);
						}
					}
					break;

				case PolygonToolMode.Split:
					break;

				default:
					break;
			}

			if ((mode == PolygonToolMode.None) || (mode == PolygonToolMode.Add))
			{
				if (keys.HasFlag(Keys.Shift))
				{
					Point[] surroundingPoints;
					if (TryFindClosedLoopSurroundingPoint(imageHotSpot, out surroundingPoints))
					{
						var clientPoints = surroundingPoints.Select(p => canvas.ImageToClient(p)).ToArray();

						Brush brush = canvas.validPolygonBrush;
						Pen pen = canvas.validPolygonEdgePen;

						graphics.FillPolygon(brush, clientPoints);
						graphics.DrawPolygon(pen, clientPoints);
					}
				}
			}
		}

		private Polygon FindNearestPolygonIntersectingLine(Point startPoint, Point endPoint)
		{
			// Find a polygon that we can see from the start point
			// Do this by tracing a line out from the start point to the end point
			// then loop through all polygons and find the closest polygon that
			// intersects with the rayPoint nearestIntersection;
			Point nearestIntersection;
			double nearestDistance = double.MaxValue;
			Point nearestPolygonEdgePoint1 = Point.Empty;
			Point nearestPolygonEdgePoint2 = Point.Empty;
			Polygon nearestPolygon = null;

			foreach (var polygon in Image.Polygons)
			{
				PointF intersection;

				for (var index = 0; index < polygon.Points.Count; index++)
				{
					var currPoint = polygon.Points[index];
					var nextPoint = polygon.Points[(index + 1) % polygon.Points.Count];

					if (GeometeryUtils.FindLineIntersection(
						startPoint,
						endPoint,
						currPoint.Point,
						nextPoint.Point,
						out intersection))
					{
						double distance = GeometeryUtils.DistanceBetween(startPoint, intersection.Round());

						if (distance < nearestDistance)
						{
							nearestDistance = distance;
							nearestIntersection = intersection.Round();

							nearestPolygonEdgePoint1 = currPoint.Point;
							nearestPolygonEdgePoint2 = nextPoint.Point;

							nearestPolygon = polygon;
						}
					}
				}
			}

			return nearestPolygon;
		}

		private Point[] FindSharedPointsBetweenPolygons(Polygon polygon1, Polygon polygon2)
		{
			var sharedPoints = new List<Point>();

			foreach (var polygon1Point in polygon1.Points)
			{
				foreach (var polygon2Point in polygon2.Points)
				{
					if (polygon1Point.Point.Equals(polygon2Point.Point))
					{
						sharedPoints.Add(polygon1Point);
					}
				}
			}

			return sharedPoints.ToArray();
		}

		private bool TryFindClosedLoopSurroundingPoint(Point point, out Point[] points)
		{
			// Get the top-left-most point of all the points in our polygons
			var topLeft = new Point(
				Image.Polygons.SelectMany(p => p.Points).Min(p => p.X - 1),
				Image.Polygons.SelectMany(p => p.Points).Min(p => p.Y - 1));

			Polygon nearestPolygon = FindNearestPolygonIntersectingLine(point, topLeft);

			if (nearestPolygon == null)
			{
				points = null;
				return false;
			}

			var surroundingPolygonsList =
				(from polygon1 in canvas.Image.Polygons
				 from polygon2 in canvas.Image.Polygons
				 where
					 (polygon1 != polygon2)
					 && (polygon1 != nearestPolygon)
					 && (polygon2 != nearestPolygon)

					 // and Polygon1 shares a point with the Nearest Polygon
					 && (polygon1.Points.Any(p => nearestPolygon.Points.Contains(p.Point)))

					 // and Polygon1 shares a point with Polygon2
					 && (polygon1.Points.Any(p => polygon2.Points.Contains(p.Point)))

					 // and Polygon2 shares a point with Nearest Polygon
					 && (polygon2.Points.Any(p => nearestPolygon.Points.Contains(p)))

				 select new[] { nearestPolygon, polygon1, polygon2 })
				 .ToList();

			var surroundingPoints = new Point[3];

			foreach (var polygons in surroundingPolygonsList)
			{
				bool validList = true;

				for (var polygonIndex = 0; polygonIndex < 3; polygonIndex++)
				{
					var sharedPoints = FindSharedPointsBetweenPolygons(
						polygons[polygonIndex],
						polygons[(polygonIndex + 1) % 3]);

					if (sharedPoints.Length == 1)
					{
						surroundingPoints[polygonIndex] = sharedPoints[0];
					}
					else
					{
						validList = false;
						break;
					}
				}

				if (validList)
				{
					if (surroundingPoints[0].Equals(surroundingPoints[1]) ||
						surroundingPoints[1].Equals(surroundingPoints[2]))
					{
						validList = false;
					}
				}

				if (validList)
				{
					validList = GeometeryUtils.IsPointInPolygonUsingRayTrace(ref surroundingPoints, point);
				}

				if (validList)
				{
					points = surroundingPoints;
					return true;
				}
			}

			points = null;
			return false;

			/*
			var sb = new System.Text.StringBuilder();
			foreach (var p in surroundingPolygons.SelectMany(p => p.Points))
			{
				sb.AppendLine(string.Format("{0}\t{1}\t{0},{1}", p.X, p.Y));
			}
			System.Windows.Forms.Clipboard.SetText(sb.ToString());
			*/
		}

		protected internal override void MouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			mouseDownImagePoint = canvas.GetImageHotSpotSnapped(true);

			// Work out which of the following we're doing:
			//  - Resizing a polygon (where we've grabbed one of it's points)
			//  - Splitting a polygon (where we've clicked inside a polygon)
			//  - Adding a new polygon
			if (e.Button == MouseButtons.Left)
			{
				var resizingPolygon = Image.Polygons.FirstOrDefault(
					p => p.Points.Contains(mouseDownImagePoint.Round()));

				if (resizingPolygon != null)
					mode = PolygonToolMode.Resize;
				else
				{
					var splittingPolygon = Image.Polygons.FirstOrDefault(
						p => p.ContainsPoint(mouseDownImagePoint.Round()));

					if (splittingPolygon != null)
					{
						mode = PolygonToolMode.Split;
						mouseDownPolygon = splittingPolygon;
					}
					else
						mode = PolygonToolMode.Add;
				}
			}

			CalculateClosestPoints(true);
		}

		private Point[] closestAddPoints = new Point[0];
		private bool canAddPolygon = false;
		private Point lastAddCalcImagePoint = new Point(int.MinValue, int.MinValue);

		private void CalculateClosestPoints(bool forceRecalc)
		{
			if (mode != PolygonToolMode.Add)
			{
				lastAddCalcImagePoint = new Point(int.MinValue, int.MinValue);
				canAddPolygon = false;
				return;
			}

			Point imageHotSpot = GetImageHotSpotSnapped().Round();

			if (!forceRecalc)
			{
				if ((imageHotSpot.X == lastAddCalcImagePoint.X) && (imageHotSpot.Y == lastAddCalcImagePoint.Y))
					return;
			}
			
			lastAddCalcImagePoint = imageHotSpot;

			if (Image.Polygons.Count == 0)
			{
				closestAddPoints = firstPolygonClientPoints != null ? firstPolygonClientPoints.Take(2).ToArray() : new Point[0];
				canAddPolygon = true;
				return;
			}

			// Find the closest 2 points, from our existing polygons,
			// that do not intersect with polygons along the way
			closestAddPoints = Image.Polygons.GetAllPoints()
				.Distinct()
				.OrderBy(loopPoint => loopPoint.DistanceFrom(mouseDownImagePoint))
				.Where(loopPoint =>
				{
					foreach (var polygon in Image.Polygons)
					{
						if (GeometeryUtils.IsLineIntersectingPolygon(
							mouseDownImagePoint.Round(),
							loopPoint,
							polygon.Points.Select(p => p.Point).ToArray(),
							true))
							return false;
					}
					return true;
				})
				.Take(2)
				.Select(p => ImageToClient(p).Round())
				.ToArray();

			if (closestAddPoints.Length == 2)
			{
				bool canAdd = true;
				foreach (var polygon in Image.Polygons)
				{
					foreach (var loopPoint in closestAddPoints)
					{
						if (GeometeryUtils.IsLineIntersectingPolygon(
							imageHotSpot,
							ClientToImage(loopPoint).Round(),
							polygon.Points.Select(p => p.Point).ToArray(),
							true))
						{
							canAdd = false;
							break;
						}
					}
					if (!canAdd)
						break;
				}
				canAddPolygon = canAdd;
			}
			else
				canAddPolygon = false;
		}

		protected internal override void MouseMove(MouseEventArgs e)
		{
			if (mode == PolygonToolMode.Add)
				CalculateClosestPoints(true);
		}

		protected internal override void MouseUp(MouseEventArgs e)
		{
			CalculateClosestPoints(true);

			try
			{
				if (e.Button == MouseButtons.Left)
				{
					string changeDescription = "";

					var polygonsToUpdateColorsFor = new List<Polygon>();

					try
					{
						var mouseDownImagePointRounded = mouseDownImagePoint.Round();

						var imageHotSpot = canvas.GetImagePointSnapped(
							canvas.ClientToImage(e.Location).Round(),
								mode == PolygonToolMode.Resize ?
								new Point[] { mouseDownImagePointRounded } :
								new Point[0]
							);

						switch (mode)
						{
							case PolygonToolMode.Resize:
								foreach (var polygon in Image.Polygons)
								{
									for (var pointIndex = 0; pointIndex < polygon.Points.Count; pointIndex++)
									{
										if (polygon.Points[pointIndex].Equals(mouseDownImagePointRounded))
										{
											polygon.Points[pointIndex] = new ColoredPoint(imageHotSpot);

											// Recalculate the color of this polygon
											polygon.Color = GraphicsUtils.GetAverageColor(
												Image.GetBaseImage(),
												polygon.Points.Select(p => p.Point));

											polygonsToUpdateColorsFor.Add(polygon);
										}
									}
								}

								changeDescription = "Resize polygon";
								break;

							case PolygonToolMode.Add:
								var keys = Control.ModifierKeys;

								if (keys.HasFlag(Keys.Shift))
								{
									Point[] surroundingPoints;
									if (TryFindClosedLoopSurroundingPoint(imageHotSpot, out surroundingPoints))
									{
										firstPolygonClientPoints.Clear();

										var polygon = new Polygon();
										for (var index = 0; index < 3; index++)
										{
											polygon.Points.Add(new ColoredPoint(surroundingPoints[index]));
										}
										polygon.Color = GraphicsUtils.GetAverageColor(
											Image.GetBaseImage(),
											polygon.Points.Select(p => p.Point));
										Image.Polygons.Add(polygon);
										polygonsToUpdateColorsFor.Add(polygon);
										changeDescription = "Add polygon";
									}
								}
								else
								{
									Point[] closestPoints;

									if (Image.Polygons.Count == 0)
									{
										closestPoints = closestAddPoints
											.Select(p => ClientToImage(p).Round())
											.Take(2)
											.ToArray();
									}
									else
									{
										// Find the closest 2 points, from our existing polygons,
										// that do not intersect with polygons along the way
										closestPoints = Image.Polygons.GetAllPoints()
											.Distinct()
											.OrderBy(loopPoint => loopPoint.DistanceFrom(mouseDownImagePoint))
											.Where(loopPoint =>
											{
												foreach (var polygon in Image.Polygons)
												{
													if (GeometeryUtils.IsLineIntersectingPolygon(mouseDownImagePointRounded, loopPoint,
														polygon.Points.Select(p => p.Point).ToArray(), true))
														return false;
												}
												return true;
											})
											.Take(2)
											.ToArray();
									}

									if (closestPoints.Length == 2)
									{
										bool canAdd = true;
										foreach (var polygon in Image.Polygons)
										{
											foreach (var loopPoint in closestPoints)
											{
												if (GeometeryUtils.IsLineIntersectingPolygon(
													imageHotSpot,
													loopPoint,
													polygon.Points.Select(p => p.Point).ToArray(),
													true))
												{
													canAdd = false;
													break;
												}
											}
											if (!canAdd)
												break;
										}

										if (canAdd)
										{
											var polygon = new Polygon();
											polygon.Points.Add(new ColoredPoint(imageHotSpot));
											polygon.Points.Add(new ColoredPoint(closestPoints.ElementAt(0)));
											polygon.Points.Add(new ColoredPoint(closestPoints.ElementAt(1)));
											polygon.Color = GraphicsUtils.GetAverageColor(
												Image.GetBaseImage(),
												polygon.Points.Select(p => p.Point));
											Image.Polygons.Add(polygon);
											polygonsToUpdateColorsFor.Add(polygon);
											changeDescription = "Add polygon";

											firstPolygonClientPoints.Clear();
										}
									}
									else
									{
										if (Image.Polygons.Count == 0)
										{
											firstPolygonClientPoints.Add(ImageToClient(imageHotSpot).Round());
										}
									}
								}
								break;

							case PolygonToolMode.Split:
								var polygonToSplit = Image.Polygons.FirstOrDefault(p => p.ContainsPoint(mouseDownImagePoint.Round()));
								var originalPolygonToSplit = Image.Polygons.FirstOrDefault(p => p.ContainsPoint(imageHotSpot));

								if (polygonToSplit == originalPolygonToSplit)
								{
									// Remove this polygon
									Image.Polygons.Remove(polygonToSplit);

									// And create 3 new polygons in it's place
									for (var pointIndex = 0; pointIndex < polygonToSplit.Points.Count; pointIndex++)
									{
										Point p1 = polygonToSplit.Points[pointIndex];
										Point p2 = polygonToSplit.Points[(pointIndex + 1) % polygonToSplit.Points.Count];

										var newPolygon = new Polygon();
										newPolygon.Points.Add(new ColoredPoint(p1));
										newPolygon.Points.Add(new ColoredPoint(p2));
										newPolygon.Points.Add(new ColoredPoint(imageHotSpot));
										newPolygon.Color = GraphicsUtils.GetAverageColor(
											Image.GetBaseImage(),
											newPolygon.Points.Select(p => p.Point));
										Image.Polygons.Add(newPolygon);

										polygonsToUpdateColorsFor.Add(newPolygon);
									}
									changeDescription = "Split polygon";
								}

								break;

							default:
								break;
						}
					}
					finally
					{
						foreach (var polygon in polygonsToUpdateColorsFor)
						{
							Image.UpdatePolygonPointColors(polygon);
						}

						if (changeDescription != "")
							canvas.MadeChanges(changeDescription);
					}
				}
			}
			finally
			{
				if (e.Button == MouseButtons.Left)
					mode = PolygonToolMode.None;
			}
		}

		internal protected override Point[] GetImagePointsNotToSnapTo()
		{
			return new Point[] { mouseDownImagePoint.Round() };
		}

		public override Cursor Cursor
		{
			get
			{
				return Cursors.CustomCursors.Cross;
			}
		}
	}
}