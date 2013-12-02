using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Polygen;
using Polygen.Schema;
using Polygen.Tools;
using System.ComponentModel;
using KdTree;
using KdTree.Math;

namespace Polygen.UI
{
	public delegate void ZoomChangedEvent(object sender, EventArgs e);

	public delegate void ChangesMadeEvent(object sender, EventArgs e);
	
	public class ImageCanvas : Control
	{
		public ImageCanvas()
		{
			creating = true;
			try
			{
				this.DoubleBuffered = true;
				this.SetStyle(ControlStyles.UserPaint |
							  ControlStyles.AllPaintingInWmPaint |
							  ControlStyles.ResizeRedraw |
							  ControlStyles.OptimizedDoubleBuffer |
							  ControlStyles.SupportsTransparentBackColor,
							  true);

				Width = 200;
				Height = 100;

				vertScrollbar = new VScrollBar();
				vertScrollbar.Parent = this;

				horzScrollbar = new HScrollBar();
				horzScrollbar.Parent = this;

				pnlScrollbarCorner = new Panel();
				pnlScrollbarCorner.Parent = this;

				vertScrollbar.Location = new Point(ClientSize.Width - vertScrollbar.Width, 0);
				vertScrollbar.Height = ClientSize.Height - horzScrollbar.Height;
				vertScrollbar.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
				vertScrollbar.Scroll += vertScrollbar_Scroll;
				vertScrollbar.SmallChange = 10;
				vertScrollbar.LargeChange = 30;

				horzScrollbar.Location = new Point(0, ClientSize.Height - horzScrollbar.Height);
				horzScrollbar.Width = ClientSize.Width - vertScrollbar.Width;
				horzScrollbar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
				horzScrollbar.Scroll += horzScrollbar_Scroll;
				horzScrollbar.SmallChange = 10;
				horzScrollbar.LargeChange = 30;

				pnlScrollbarCorner.Location = new Point(
					horzScrollbar.Right,
					vertScrollbar.Bottom);
				pnlScrollbarCorner.Size = new Size(vertScrollbar.Width, horzScrollbar.Height);
				pnlScrollbarCorner.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

				Snapping = true;
				ShowPolygonOutlines = true;

				UpdateScrollbars();

				validPolygonEdgePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
				validPolygonEdgePen.DashPattern = new float[] { 5, 5 };

				invalidPolygonEdgePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
				invalidPolygonEdgePen.DashPattern = new float[] { 5, 5 };

				repaintTimer.Interval = 1;
				repaintTimer.Enabled = false;
				repaintTimer.Tick += RepaintTimerTick;
			}
			finally
			{
				creating = false;
			}
		}

		private bool creating = false;

		private KdTree<float, Polygon> pointTree = new KdTree<float, Polygon>(2, new FloatMath());

		public void ReloadPointTree()
		{
			pointTree.Clear();

			if (Image == null)
				return;

			foreach (var polygon in Image.Polygons)
			{
				foreach (var point in polygon.Points)
				{
					pointTree.Add(new float[] { point.X, point.Y }, polygon);
				}
			}
		}

		private void horzScrollbar_Scroll(object sender, ScrollEventArgs e)
		{
			if (updatingScrollbars)
				return;

			ScrollingAreaLocation = new Point(
				RealClientRect.Left - e.NewValue,
				ScrollingAreaLocation.Y
			);

			UpdateScrollbars();

			Invalidate();
		}
		
		private void vertScrollbar_Scroll(object sender, ScrollEventArgs e)
		{
			if (updatingScrollbars)
				return;

			ScrollingAreaLocation = new Point(
				ScrollingAreaLocation.X,
				RealClientRect.Top - e.NewValue
			);

			UpdateScrollbars();

			Invalidate();
		}

		private HScrollBar horzScrollbar;
		private VScrollBar vertScrollbar;
		private Panel pnlScrollbarCorner;

		private ImageDef image = null;
		public ImageDef Image
		{
			get { return image; }
			set
			{
				if (value == image)
					return;
				
				image = value;

				ReloadPointTree();
				
				undoManager.Reset();
				if (image != null)
					undoManager.Init(image.ToBytes());

				if (ClientRectangle.Contains(PointToClient(MousePosition)))
				{
					if (image != null)
						ShowMouseCursor();
					else
						HideMouseCursor();
				}

				ResetZoomAndCenter();
			}
		}

		private double zoom = 100;
		public double Zoom
		{
			get { return zoom; }
			set
			{
				if (value == zoom)
					return;

				var clientHotSpot = PointToClient(MousePosition);

				var clientZoomCenter = GetZoomFocalPoint(clientHotSpot);

				var imageZoomCenter = ClientToImage(clientZoomCenter);

				zoom = value;

				var newClientZoomCenter = ImageToClient(imageZoomCenter);

				var clientOffset = clientZoomCenter.Subtract(newClientZoomCenter);

				imageClientLocation.Offset(clientOffset.Round());

				var testImageZoomCenter = ClientToImage(clientZoomCenter);

				CenterImageIfNeeded();

				UpdateScrollbars();

				ZoomChanged(this, new EventArgs());

				Invalidate();
			}
		}

		private void CenterImageIfNeeded()
		{
			// Center Image (on Y axis) if need be
			if (ScrollingAreaSize.Height < RealClientRect.Height)
			{
				ScrollingAreaLocation = new Point(
					ScrollingAreaLocation.X,
					RealClientRect.Top + (RealClientRect.Height / 2) - (ScrollingAreaSize.Height / 2));
			}
			else
			{
				if (ScrollingAreaLocation.Y > RealClientRect.Y)
					ScrollingAreaLocation = new Point(
						ScrollingAreaLocation.X,
						RealClientRect.Y);
				else if (ScrollingAreaRect.Bottom < RealClientRect.Bottom)
					ScrollingAreaLocation = new Point(
						ScrollingAreaLocation.X,
						RealClientRect.Bottom - ScrollingAreaSize.Height);


				if (ScrollingAreaLocation.X > RealClientRect.X)
					ScrollingAreaLocation = new Point(
						RealClientRect.X,
						ScrollingAreaLocation.Y);
				else if (ScrollingAreaRect.Right < RealClientRect.Right)
					ScrollingAreaLocation = new Point(
						RealClientRect.Right - ScrollingAreaSize.Width,
						ScrollingAreaLocation.Y);
			}

			// Center Image (on X axis) if need be
			if (ScrollingAreaSize.Width < RealClientRect.Width)
			{
				ScrollingAreaLocation = new Point(
					RealClientRect.Left + (RealClientRect.Width / 2) - (ScrollingAreaSize.Width / 2),
					ScrollingAreaLocation.Y);
			}
		}

		public event ZoomChangedEvent ZoomChanged;

		private Point GetZoomFocalPoint(Point clientHotSpot)
		{
			if (RealClientRect.Contains(clientHotSpot))
				return clientHotSpot.ClipTo(ImageClientRect);
			else
				return RealClientRect.GetCenter().Round();
		}

		private Point GetZoomFocalPoint()
		{
			var clientHotSpot = PointToClient(MousePosition);
			return GetZoomFocalPoint(clientHotSpot);
		}

		private Point imageClientLocation = Point.Empty;

		private Rectangle ImageClientRect
		{
			get
			{
				return new Rectangle(imageClientLocation, ImageClientSize);
			}
		}
		
		// This is the real client rect (ClientRect minus the space occupied by Scrollbars if they are visible)
		private Rectangle RealClientRect
		{
			get
			{
				return new Rectangle(0, 0,
					vertScrollbar.Visible ? ClientRectangle.Width - vertScrollbar.Width : ClientRectangle.Width,
					horzScrollbar.Visible ? ClientRectangle.Height - horzScrollbar.Height : ClientRectangle.Height);
			}
		}

		// Size of the Image Area translated to client coordinates
		private Size ImageClientSize
		{
			get
			{
				if (image == null)
					return new Size(0, 0);

				double zoomFactor = zoom / 100;

				return new Size(
					(int)Math.Round(image.Width * zoomFactor),
					(int)Math.Round(image.Height * zoomFactor));
			}
		}

		private const int scrollingAreaPadding = 10;

		// Size of the area that scrolls up and down
		private Size ScrollingAreaSize
		{
			get
			{
				// By my logic it should be scrollingAreaPadding * 2 (1 padding on each side) 
				// but that doesn't seem to work, couldn't work out why, but * 3 does seem to work.
				return ImageClientSize.Inflate(scrollingAreaPadding * 3, scrollingAreaPadding * 3);
			}
		}

		private Point ScrollingAreaLocation
		{
			get
			{
				var point = imageClientLocation;
				point.Offset(-scrollingAreaPadding, -scrollingAreaPadding);
				return point;
			}
			set
			{
				imageClientLocation = new Point(
					value.X + scrollingAreaPadding,
					value.Y + scrollingAreaPadding);
			}
		}

		private Rectangle ScrollingAreaRect
		{
			get
			{
				return new Rectangle(
					ScrollingAreaLocation,
					ScrollingAreaSize);
			}
		}

		private bool updatingScrollbars = false;

		private void UpdateScrollbars()
		{
			updatingScrollbars = true;
			try
			{
				if (image == null)
				{
					horzScrollbar.Visible = false;
					vertScrollbar.Visible = false;
					pnlScrollbarCorner.Visible = false;
					return;
				}

				// Do we need to show the scrollbars?
				horzScrollbar.Visible = ScrollingAreaSize.Width > ClientRectangle.Width;
				vertScrollbar.Visible = ScrollingAreaSize.Height > ClientRectangle.Height;
				pnlScrollbarCorner.Visible = horzScrollbar.Visible || vertScrollbar.Visible;

				if (horzScrollbar.Visible)
				{
					UpdateScrollbar(
						horzScrollbar,
						0,
						ScrollingAreaSize.Width - RealClientRect.Width,
						RealClientRect.Left - ScrollingAreaLocation.X);
				}

				if (vertScrollbar.Visible)
				{
					UpdateScrollbar(
						vertScrollbar,
						0,
						ScrollingAreaSize.Height - RealClientRect.Height,
						RealClientRect.Top - ScrollingAreaLocation.Y);
				}
			}
			finally
			{
				updatingScrollbars = false;
			}
		}

		private void UpdateScrollbar(ScrollBar scrollbar, int min, int max, int value)
		{
			if ((scrollbar.Minimum != min) || (scrollbar.Maximum != max) || (scrollbar.Value != value))
			{
				scrollbar.Minimum = int.MinValue;
				scrollbar.Maximum = int.MaxValue;
				scrollbar.Value = value;
				scrollbar.Minimum = min;
				scrollbar.Maximum = max;
			}
		}

		public void ResetZoomAndCenter()
		{
			UpdateScrollbars();

			zoom = 100;

			var realClientRectCenter = RealClientRect.GetCenter();
			imageClientLocation.X = (int)Math.Round(realClientRectCenter.X - (ImageClientSize.Width / 2));
			imageClientLocation.Y = (int)Math.Round(realClientRectCenter.Y - (ImageClientSize.Height / 2));

			UpdateScrollbars();

			Invalidate();
		}

		public ColoredPointF ClientToImage(ColoredPointF clientPoint)
		{
			float zoomFactor = (float)Zoom / 100;

			var translated = new ColoredPointF();

			translated.X = ((clientPoint.X - imageClientLocation.X) / zoomFactor);
			translated.Y = ((clientPoint.Y - imageClientLocation.Y) / zoomFactor);

			return translated;
		}

		public ColoredPointF ImageToClient(ColoredPointF imagePoint)
		{
			float zoomFactor = (float)Zoom / 100;

			var translated = new ColoredPointF();

			translated.X = imageClientLocation.X + (imagePoint.X * zoomFactor);
			translated.Y = imageClientLocation.Y + (imagePoint.Y * zoomFactor);

			return translated;
		}

		public PointF ClientToImage(PointF clientPoint)
		{
			float zoomFactor = (float)Zoom / 100;

			var translated = new PointF();

			translated.X = ((clientPoint.X - imageClientLocation.X) / zoomFactor);
			translated.Y = ((clientPoint.Y - imageClientLocation.Y) / zoomFactor);

			return translated;
		}

		public PointF ImageToClient(PointF imagePoint)
		{
			float zoomFactor = (float)Zoom / 100;

			var translated = new PointF();

			translated.X = imageClientLocation.X + (imagePoint.X * zoomFactor);
			translated.Y = imageClientLocation.Y + (imagePoint.Y * zoomFactor);

			return translated;
		}

		public bool Snapping { get; set; }

		private Point GetClientPointSnapped(Point point)
		{
			return GetClientPointSnapped(point, new Point[0]);
		}

		private Point GetClientPointSnapped(Point point, IEnumerable<Point> imagePointsNotToSnapTo)
		{
			if (!Snapping)
				return point;

			// Find the closest point within snapping distance from point
			var points = image.Polygons.GetAllPoints()
				.Select(p => ImageToClient(p).Round())
				.Where(p =>
					(p.DistanceFrom(point) <= handleSnappingDistance) &&
					(!imagePointsNotToSnapTo.Contains(ClientToImage(p).Round())))
				.OrderBy(p => p.DistanceFrom(point))
				.Take(1)
				.ToArray();

			// And return it
			if (points.Length > 0)
				return points[0];
			else
				return point;
		}

		public Point GetImagePointSnapped(Point point)
		{
			return GetImagePointSnapped(point, new Point[0]);
		}

		internal Point GetImagePointSnapped(Point point, IEnumerable<Point> pointsNotToSnapTo)
		{
			if (!Snapping)
				return point;

			// Find the closest point within snapping distance from point
			var points =
				pointTree.RadialSearch(new float[] { point.X, point.Y }, handleSnappingDistance, 100)
				.Select(node => new Point((int)node.Point[0], (int)node.Point[1]))
				.Where(p => !pointsNotToSnapTo.Contains(p))
				.ToArray();

			// And return it
			if (points.Length > 0)
				return points[0];
			else
				return point;
		}

		/*
		internal Point GetImagePointSnapped(Point point, IEnumerable<Point> pointsNotToSnapTo)
		{
			if (!Snapping)
				return point;

			// Find the closest point within snapping distance from point
			var points = image.Polygons.GetAllPoints()
				.Where(p =>
					(p.DistanceFrom(point) <= handleSnappingDistance) &&
					(!pointsNotToSnapTo.Contains(p)))
				.OrderBy(p => p.DistanceFrom(point))
				.Take(1)
				.ToArray();

			// And return it
			if (points.Length > 0)
				return points[0];
			else
				return point;
		}
		*/

		private const int handleRadius = 3;
		private const int handleSnappingDistance = 7;

		internal Brush validPolygonBrush = new SolidBrush(Color.FromArgb(70, Color.Green));
		internal Pen validPolygonEdgePen = new Pen(Color.Green);
		//internal Pen newPolygonEdgePen = new Pen(Color.Green, 1);

		internal Brush handleHotBrush = new SolidBrush(Color.Green);
		internal Brush handleNormalBrush = new SolidBrush(Color.LightBlue);

		internal Brush invalidPolygonBrush = new SolidBrush(Color.FromArgb(70, Color.Red));
		internal Pen invalidPolygonEdgePen = new Pen(Color.Red, 1);

		protected override void OnPaint(PaintEventArgs e)
		{
			// Fill the background
			e.Graphics.FillRectangle(Brushes.DarkGray, ClientRectangle);
			//e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			// If we're running in design time then don't do anymore drawing
			if (DesignMode || (image == null))
			{
				horzScrollbar.Visible = false;
				vertScrollbar.Visible = false;
				pnlScrollbarCorner.Visible = false;
				return;
			}

			try
			{
				// Draw the image
				DrawImage(e.Graphics);

				// Draw the polygons
				DrawPolygons(e.Graphics);

				// Finally draw the mouse pointer
				DrawCursor(e.Graphics);

				//DrawDebugText(e.Graphics, Enum.GetName(typeof(ToolAction), mouseDownAction));
				
				if (ShowFps)
				{
					DrawFps(e.Graphics);
				}
			}
			catch (Exception error)
			{
				DrawError(e.Graphics, error);
			}

			repaintTimer.Enabled = !DesignMode;
		}

		private void DrawError(Graphics graphics, Exception error)
		{
			graphics.FillRectangle(Brushes.White, ImageClientRect);

			var text = "Error:\n" + error.Message + "\n\n" + error.StackTrace;

			var font = new System.Drawing.Font("Arial", 8);

			graphics.DrawString(text, font, Brushes.Red, new Point(5, 5));
		}

		private Timer repaintTimer = new Timer();
		private int ticksSinceLastFpsMeasure = int.MinValue;
		private int repaintCountSinceLastFpsMeasure = 0;
		private int currentFps = -1;
		private Random random = new Random();

		private void DrawCheckerboard(Graphics graphics, int opacity)
		{
			var rect = ImageClientRect;

			var tileRect = new Rectangle();
			tileRect.Location = rect.Location;

			const int tileSize = 10;

			tileRect.Width = tileSize;
			tileRect.Height = tileSize;

			Brush[] brushes = new Brush[]
			{
				new SolidBrush(Color.FromArgb(opacity, Color.White)),
				new SolidBrush(Color.FromArgb(opacity, Color.DarkGray))
			};

			Point tileNumber = new Point(0, 0);

			do
			{
				if (tileRect.Bottom > rect.Bottom)
					tileRect.Height = rect.Bottom - tileRect.Top + 1;

				do
				{
					var brush = brushes[(tileNumber.Y + tileNumber.X) % 2];

					if (tileRect.Right > rect.Right)
						tileRect.Width = rect.Right - tileRect.Left + 1;

					graphics.FillRectangle(brush, tileRect);

					tileRect.X += tileRect.Width;
					tileNumber.X += 1;
				}
				while (tileRect.Left < rect.Right);

				tileRect.X = rect.X;
				tileRect.Y += tileRect.Height;
				tileRect.Width = tileSize;

				tileNumber.X = 0;
				tileNumber.Y += 1;
			}
			while (tileRect.Top <= rect.Bottom);
		}

		private void DrawFps(Graphics graphics)
		{
			repaintCountSinceLastFpsMeasure++;

			if (ticksSinceLastFpsMeasure == int.MinValue)
			{
				ticksSinceLastFpsMeasure = Environment.TickCount;
				return;
			}

			int elapsedTicks = Environment.TickCount - ticksSinceLastFpsMeasure;

			const int elapsedTickMeasureThreshold = 500;

			if (elapsedTicks > elapsedTickMeasureThreshold)
			{
				currentFps = (int)Math.Truncate(
					repaintCountSinceLastFpsMeasure * 
					(elapsedTicks / (float)elapsedTickMeasureThreshold) * 
					(1000 / (float)elapsedTickMeasureThreshold));
				repaintCountSinceLastFpsMeasure = 0;
				ticksSinceLastFpsMeasure = Environment.TickCount;
			}

			if (currentFps != -1)
				DrawDebugText(graphics, string.Format("{0} fps", currentFps));
		}

		private void RepaintTimerTick(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void DrawCursor(Graphics graphics)
		{
			// We have to draw our own mouse cursor primarily because of 
			// the snapping - when we're snapping the cursor to a handle/edge 
			// we need to offset the drawing of the cursor from the actual cursor 
			// position, which we can only do by drawing it ourself.
			bool mouseIsDown =
				((Control.MouseButtons & MouseButtons.Left) != 0) ||
				((Control.MouseButtons & MouseButtons.Middle) != 0) ||
				((Control.MouseButtons & MouseButtons.Right) != 0);

			// We won't always want to draw a custom mouse cursor
			bool shouldDrawCustomCursor = true;
			
			// If the mouse is down and over the canvas but wasn't pressed down on 
			// the canvas (like maybe the user is scrolling a scrollbar) then don't
			// draw the cursor
			if (mouseIsDown && (mouseButtonDown == MouseButtons.None))
				shouldDrawCustomCursor = false;

			var clientHotSpot = ImageToClient(GetImageHotSpotSnapped()).Round();

			// If the mouse is outside the client area and no mouse buttons are down 
			// on the canvas then don't draw the cursor
			if ((!RealClientRect.Contains(clientHotSpot)) && (mouseButtonDown == MouseButtons.None))
				shouldDrawCustomCursor = false;

			var cursor = activeTool != null ? activeTool.Cursor : System.Windows.Forms.Cursors.Default;

			if (shouldDrawCustomCursor)
			{
				var cursorRect = new Rectangle(
					clientHotSpot.X - cursor.HotSpot.X,
					clientHotSpot.Y - cursor.HotSpot.Y,
					cursor.Size.Width,
					cursor.Size.Height);

				cursor.Draw(graphics, cursorRect);
			}
		}

		internal PointF GetImageHotSpotSnapped(bool updateActualMouseToSnappedPosition = false)
		{
			var point = PointToClient(MousePosition);
			
			point = ClientToImage(point).Round();

			Point[] pointsToNotSnapTo = activeTool != null ? activeTool.GetImagePointsNotToSnapTo() : new Point[0];

			PointF pointUnsnapped = point;

			point = GetImagePointSnapped(point, pointsToNotSnapTo);

			if (updateActualMouseToSnappedPosition && (!point.Equals(pointUnsnapped)))
			{
				Point newMousePoint = ImageToClient(point).Round();
				newMousePoint = PointToScreen(newMousePoint);
				System.Windows.Forms.Cursor.Position = newMousePoint;
			}

			return point;
		}

		private MouseButtons mouseButtonDown = MouseButtons.None;
		private PointF mouseDownImagePoint = Point.Empty;
		private bool abortedMouseDown = false;

		public bool IsMouseDown
		{
			get { return mouseButtonDown != MouseButtons.None; }
		}

		private enum ToolAction
		{
			None,
			ResizingPolygon,
			NewPolygon,
			SplitPolygon
		}

		private bool hidingMouseCursor = false;

		private void DrawImage(Graphics graphics)
		{
			// Draw a shadow surrounding the image
			Brush shadowBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
			const int imageShadowSize = 3;
			graphics.FillRectangle(
				shadowBrush,
				imageClientLocation.X + imageShadowSize,
				imageClientLocation.Y + imageShadowSize,
				ImageClientSize.Width,
				ImageClientSize.Height);

			// Draw the base image underneath
			graphics.FillRectangle(Brushes.White, ImageClientRect);
			graphics.DrawImage(image.GetBaseImage().GetBitmap(), ImageClientRect);

			// Draw checkerboard
			byte overlayOpacity = (byte)(100 - imageOpacity);
			overlayOpacity = (byte)Math.Min(255, 255 * (overlayOpacity / 100f));
			DrawCheckerboard(graphics, overlayOpacity);

			// Draw border around image
			graphics.DrawRectangle(Pens.Black,
				imageClientLocation.X - 1, imageClientLocation.Y - 1,
				ImageClientSize.Width + 1, ImageClientSize.Height + 1);
		}

		internal void DrawDebugText(Graphics graphics, string debugText)
		{
			if (debugText != "")
			{
				var debugPos = new Point(0, 0);
				var font = new Font("Consolas", 10);
				RectangleF rect = new RectangleF(debugPos, graphics.MeasureString(debugText, font));
				graphics.FillRectangle(Brushes.White, rect);
				graphics.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
				graphics.DrawString(debugText, new Font("Consolas", 10), Brushes.Black, debugPos);
			}
		}

		internal void DefaultDrawPolygon(Graphics graphics, Polygon polygon)
		{
			var points =
				polygon.Points
				.Select(point => ImageToClient(point.Point))
				.ToArray();

			if (points.Length > 2)
			{
				if (image.Shading)
				{
					var path = new System.Drawing.Drawing2D.GraphicsPath();
					path.AddPolygon(points);

					var brush = new System.Drawing.Drawing2D.PathGradientBrush(points);
					
					brush.SurroundColors = polygon.Points.Select(p => (Color)p.Color).ToArray();

					brush.CenterColor = 
						//polygon.Color;
						GraphicsUtils.BlendColors(brush.SurroundColors);

					graphics.FillPath(brush, path);
				}
				else
				{
					graphics.FillPolygon(new SolidBrush(polygon.Color), points);
				}

				if (showPolygonOutlines)
					graphics.DrawPolygon(Pens.Black, points);

				//DrawPolygonCenter(graphics, points);
			}
		}

		internal void DefaultDrawPolygonShaded(Graphics graphics, Polygon polygon)
		{
			var points = polygon.Points
				.Select(point => ImageToClient(point.Point))
				.ToArray();

			if (points.Length > 2)
			{
				var path = new System.Drawing.Drawing2D.GraphicsPath();
				path.AddLines(points);
				path.CloseFigure();

				var brush = new System.Drawing.Drawing2D.PathGradientBrush(points);
				brush.SurroundColors = polygon.Points.Select(p => (Color)p.Color).ToArray();
				brush.CenterColor = GraphicsUtils.BlendColors(brush.SurroundColors);
				//brush.CenterColor = polygon.Color;

				graphics.FillPath(brush, path);

				if (showPolygonOutlines)
					graphics.DrawPolygon(Pens.Black, points);
			}
		}

		internal void DrawPolygonCenter(Graphics graphics, PointF[] corners)
		{
			var center = GeometeryUtils.GetPolygonCenter(corners).Round();

			int radius = 3;

			graphics.DrawLine(
				Pens.Red,
				new Point(center.X - radius, center.Y - radius),
				new Point(center.X + radius, center.Y + radius));

			graphics.DrawLine(
				Pens.Red,
				new Point(center.X - radius, center.Y + radius),
				new Point(center.X + radius, center.Y - radius));
		}

		private void DrawPolygons(Graphics graphics)
		{
			// Fill our polygons
			foreach (var polygon in image.Polygons)
			{
				if (activeTool != null)
					activeTool.DrawPolygon(graphics, polygon);
				else
					DefaultDrawPolygon(graphics, polygon);
			}

			// Now draw the handles
			/*
				foreach (var polygon in image.Polygons)
				{
					// Translate the points
					var points = polygon.Points.Select<Point, Point>(
						point => ImageToClient(point).Round()
					).ToArray();

					// Fill our handles
					foreach (var point in points)
					{
						graphics.FillCircle(Brushes.White, handleNormalBrush, point, handleRadius, handleRadius - 1);
					}
				}
			*/

			if (activeTool != null)
				activeTool.AfterDrawnPolygons(graphics);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Control)
				Zoom = Math.Max(1, Zoom + (e.Delta / 12));
			else if (Control.ModifierKeys == Keys.Shift)
				ScrollByPage(horzScrollbar, -e.Delta / 12);
			else if (Control.ModifierKeys == Keys.None)
				ScrollByPage(vertScrollbar, -e.Delta / 12);

			base.OnMouseWheel(e);
		}

		private void ScrollByPage(ScrollBar scrollbar, int pageCount)
		{
			int value = scrollbar.Value + (scrollbar.SmallChange * pageCount);
			value = Math.Min(scrollbar.Maximum, value);
			value = Math.Max(scrollbar.Minimum, value);
			scrollbar.Value = value;

			if (scrollbar == vertScrollbar)
				vertScrollbar_Scroll(vertScrollbar, new ScrollEventArgs(
					pageCount > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement,
					value));
			else
				horzScrollbar_Scroll(horzScrollbar, new ScrollEventArgs(
					pageCount > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement,
					value));

			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == Keys.D0))
				ResetZoomAndCenter();

			if ((e.KeyCode == Keys.Escape) && (mouseButtonDown != MouseButtons.None))
				abortedMouseDown = true;

			base.OnKeyDown(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (e.KeyChar == '+')
				Zoom = Math.Max(1, Zoom + 10);
			else if (e.KeyChar == '-')
				Zoom = Math.Max(1, Zoom - 10);
			base.OnKeyPress(e);
		}

		private void HideMouseCursor()
		{
			if (hidingMouseCursor)
				return;

			Cursor.Hide();
			hidingMouseCursor = true;
		}

		private void ShowMouseCursor()
		{
			if (!hidingMouseCursor)
				return;

			Cursor.Show();
			hidingMouseCursor = false;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			try
			{
				if (image != null)
				{
					HideMouseCursor();
					Focus();
				}
			}
			finally
			{
				Invalidate();
				base.OnMouseEnter(e);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			try
			{
				if (image != null)
					ShowMouseCursor();
			}
			finally
			{
				Invalidate();
				base.OnMouseLeave(e);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			try
			{
				if (!creating)
				{
					CenterImageIfNeeded();
					UpdateScrollbars();
					Invalidate();
				}
			}
			finally
			{
				base.OnResize(e);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			try
			{
				if (this.GetChildAtPoint(e.Location) != null)
					return;

				mouseButtonDown = e.Button;
				mouseDownImagePoint = GetImageHotSpotSnapped(true);

				if (activeTool != null)
					activeTool.MouseDown(e);
			}
			finally
			{
				base.OnMouseDown(e);
				Invalidate();			
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			try
			{
				if (Image == null)
					return;

				if (abortedMouseDown)
					return;

				if (activeTool != null)
					activeTool.MouseUp(e);
			}
			finally
			{
				if (mouseButtonDown == e.Button)
				{
					mouseButtonDown = MouseButtons.None;
					abortedMouseDown = false;
				}

				base.OnMouseDown(e);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (activeTool != null)
				activeTool.MouseMove(e);

			Invalidate();

			base.OnMouseMove(e);
		}

		private UndoManager<byte[]> undoManager = new UndoManager<byte[]>();

		public bool CanUndo()
		{
			return undoManager.CanUndo();
		}

		public bool CanRedo()
		{
			return undoManager.CanRedo();
		}

		public string GetUndoDescription()
		{
			return undoManager.GetUndoChangeDescription();
		}

		public string GetRedoDescription()
		{
			return undoManager.GetRedoChangeDescription();
		}

		public void Undo()
		{
			image = ImageDef.FromBytes(undoManager.Undo());

			UpdateScrollbars();
			Invalidate();
		}

		public void Redo()
		{
			image = ImageDef.FromBytes(undoManager.Redo());

			UpdateScrollbars();
			Invalidate();
		}

		public void MadeChanges(string changeType)
		{
			if (Image != null)
			{
				undoManager.MadeChanges(Image.ToBytes(), changeType);
				ChangesMade(this, new EventArgs());
				ReloadPointTree();
			}
			Invalidate();
		}

		public event ChangesMadeEvent ChangesMade;

		private bool showPolygonOutlines = true;
		public bool ShowPolygonOutlines
		{
			get { return showPolygonOutlines; }
			set
			{
				if (value == showPolygonOutlines)
					return;

				showPolygonOutlines = value;
				Invalidate();
			}
		}

		private bool shadePolygons = false;
		public bool ShadePolygons
		{
			get { return shadePolygons; }
			set
			{
				if (value == shadePolygons)
					return;
				
				shadePolygons = value;

				Invalidate();
			}
		}

		// Between 1 and 100
		private byte imageOpacity = 0;
		public byte ImageOpacity 
		{
			get { return imageOpacity; }
			set
			{
				if (value == imageOpacity)
					return;

				imageOpacity = Math.Max((byte)0, Math.Min((byte)100, value));
				Invalidate();
			}
		}

		private Color backgroundColor = Color.White;
		public Color BackgroundColor
		{
			get { return backgroundColor; }
			set
			{
				if (value == backgroundColor)
					return;

				backgroundColor = value;
				Invalidate();
			}
		}

		private Tool activeTool = null;

		[Browsable(false)]
		public Type ActiveToolType
		{
			get
			{
				if (activeTool == null)
					return null;
				else
					return activeTool.GetType();
			}
			set
			{
				if (value == ActiveToolType)
					return;

				var toolType = typeof(Tool);

				if (!value.IsSubclassOf(toolType))
					throw new Exception(string.Format(
						"ActiveToolType must descend from {0}.{1}", toolType.Namespace, toolType.Name));

				activeTool = (Tool)Activator.CreateInstance(value, new object[] { this });
			}
		}

		public bool ShowFps { get; set; }

		public double CalcPolygonCoverage()
		{
			if (Image == null)
				return 0;

			double imageArea = Image.Width * Image.Height;

			if (imageArea == 0)
				return 0;

			double coverage = 0;

			foreach (var polygon in Image.Polygons)
			{
				var polygonArea = GeometeryUtils.GetPolygonArea(polygon.Points.Select(p => p.Point).ToArray());

				coverage += polygonArea;
			}

			return coverage / imageArea;
		}

		public void ExportImage(string fileName, System.Drawing.Imaging.ImageFormat format)
		{
			ImageCanvas.ExportImage(Image, fileName, format);
		}

		public static void ExportImage(ImageDef image, string fileName, System.Drawing.Imaging.ImageFormat format)
		{
			using (var bmp = new Bitmap(image.Width, image.Height))
			{
				using (var g = Graphics.FromImage(bmp))
				{
					g.FillRectangle(
						Brushes.Transparent,
						new Rectangle(0, 0, image.Width, image.Height));

					foreach (var polygon in image.Polygons)
					{
						g.FillPolygon(
							new SolidBrush(polygon.Color),
							polygon.Points.Select(p => p.Point).ToArray());
					}
				}

				bmp.Save(fileName, format);
			}
		}
	}
}