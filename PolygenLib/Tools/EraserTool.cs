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
	public class EraserTool : Tool
	{
		public EraserTool(ImageCanvas canvas)
			: base(canvas)
		{
			deletePolygonPen.DashStyle = DashStyle.Dash;
			deletePolygonPen.DashPattern = new float[] { 5, 5 };
		}

		private HatchBrush deletePolygonBrush = new HatchBrush(
			HatchStyle.DiagonalCross,
			Color.FromArgb(100, Color.Red), 
			Color.FromArgb(70, Color.Red));

		private Pen deletePolygonPen = new Pen(Color.FromArgb(150, Color.Red));

		private HatchBrush deletePolygonBrushConfirm = new HatchBrush(
			HatchStyle.DiagonalCross,
			Color.FromArgb(200, Color.Red),
			Color.FromArgb(90, Color.Red));

		private Pen deletePolygonPenConfirm = new Pen(Color.FromArgb(200, Color.Red));

		protected internal override void DrawPolygon(Graphics graphics, Polygon polygon)
		{
			var imageHotSpot = GetImageHotSpot();

			var points = polygon.Points
				.Select(p => ImageToClient(p.Point).Round())
				.ToArray();

			base.DrawPolygon(graphics, polygon);

			var isHotPolygon = polygon.ContainsPoint(GetImageHotSpot().Round());

			if (mouseButtonDown != MouseButtons.Left)
			{
				if (isHotPolygon)
				{
					graphics.FillPolygon(deletePolygonBrush, points);
					graphics.DrawPolygon(deletePolygonPen, points);
				}
			}
			else
			{
				var isMouseDownPolygon = polygon.ContainsPoint(mouseDownImagePoint.Round());

				if (isMouseDownPolygon && isHotPolygon)
				{
					graphics.FillPolygon(deletePolygonBrushConfirm, points);
					graphics.DrawPolygon(deletePolygonPen, points);
				}
				else if (isMouseDownPolygon)
				{
					graphics.FillPolygon(deletePolygonBrush, points);
					graphics.DrawPolygon(deletePolygonPen, points);
				}
			}
		}

		private PointF mouseDownImagePoint = default(PointF);

		protected internal override void MouseDown(MouseEventArgs e)
		{
			base.MouseDown(e);

			if (e.Button == mouseButtonDown)
			{
				mouseDownImagePoint = GetImageHotSpot();
			}
		}

		protected internal override void MouseUp(MouseEventArgs e)
		{
			if ((mouseButtonDown == e.Button) && (e.Button == MouseButtons.Left))
			{
				var polygonsToDelete = new List<Polygon>();

				foreach (var polygon in Image.Polygons)
				{
					var isHotPolygon = polygon.ContainsPoint(GetImageHotSpot().Round());
					var isMouseDownPolygon = polygon.ContainsPoint(mouseDownImagePoint.Round());

					if (isHotPolygon && isMouseDownPolygon)
						polygonsToDelete.Add(polygon);
				}

				if (polygonsToDelete.Count > 0)
				{
					foreach (var polygon in polygonsToDelete)
						Image.Polygons.Remove(polygon);

					canvas.MadeChanges("Delete Polygon" + ((polygonsToDelete.Count > 1) ? "s" : ""));
				}
			}

			base.MouseUp(e);
		}

		public override Cursor Cursor
		{
			get
			{
				return Cursors.CustomCursors.Eraser;
			}
		}
	}
}