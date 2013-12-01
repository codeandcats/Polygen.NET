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
	public class Tool
	{
		public Tool(ImageCanvas canvas)
		{
			this.canvas = canvas;
		}

		protected ImageCanvas canvas;

		protected ImageDef Image
		{
			get
			{
				return canvas == null ? null : canvas.Image;
			}
		}

		protected ColoredPointF ImageToClient(ColoredPointF imagePoint)
		{
			return canvas == null ? default(ColoredPointF) : canvas.ImageToClient(imagePoint);
		}

		protected PointF ImageToClient(PointF imagePoint)
		{
			return canvas == null ? default(PointF) : canvas.ImageToClient(imagePoint);
		}

		protected ColoredPointF ClientToImage(ColoredPointF clientPoint)
		{
			return canvas == null ? default(ColoredPointF) : canvas.ClientToImage(clientPoint);
		}

		protected PointF ClientToImage(PointF clientPoint)
		{
			return canvas == null ? default(PointF) : canvas.ClientToImage(clientPoint);
		}

		protected PointF GetImageHotSpotSnapped(bool updateActualMouseToSnappedPosition = false)
		{
			return canvas == null ? default(PointF) : canvas.GetImageHotSpotSnapped(updateActualMouseToSnappedPosition);
		}

		protected PointF GetImageHotSpot()
		{
			return canvas == null ? default(PointF) : ClientToImage(canvas.PointToClient(Control.MousePosition));
		}

		internal protected virtual Point[] GetImagePointsNotToSnapTo()
		{
			return new Point[0];
		}

		protected internal virtual void DrawPolygon(Graphics graphics, Polygon polygon)
		{
			canvas.DefaultDrawPolygon(graphics, polygon);
		}

		protected internal virtual void AfterDrawnPolygons(Graphics graphics)
		{
		}

		protected MouseButtons mouseButtonDown = MouseButtons.None;
		
		protected internal virtual void MouseDown(MouseEventArgs e)
		{
			if (mouseButtonDown == MouseButtons.None)
			{
				mouseButtonDown = e.Button;
			}
		}

		protected internal virtual void MouseMove(MouseEventArgs e)
		{

		}

		protected internal virtual void MouseUp(MouseEventArgs e)
		{
			if (e.Button == mouseButtonDown)
			{
				mouseButtonDown = MouseButtons.None;
			}
		}

		protected internal virtual void DrawCursor(Graphics graphics)
		{
		}

		public virtual Cursor Cursor
		{
			get
			{
				return System.Windows.Forms.Cursors.Default;
			}
		}
	}
}