using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Polygen
{
	public static class GraphicsUtils
	{
		public static string ColorToHexString(Color colour)
		{
			return
				"#" +
				colour.A.ToString("X2") +
				colour.R.ToString("X2") +
				colour.G.ToString("X2") +
				colour.B.ToString("X2");
		}

		public static Color ColorFromHexString(string hexColour)
		{
			hexColour = hexColour.Trim();

			if (hexColour.Substring(0, 1) == "#")
				hexColour = hexColour.Substring(1, hexColour.Length - 1);

			string alpha = hexColour.Substring(0, 2);
			string red = hexColour.Substring(2, 2);
			string green = hexColour.Substring(4, 2);
			string blue = hexColour.Substring(6, 2);

			return Color.FromArgb(
				Convert.ToByte(alpha, 16),
				Convert.ToByte(red, 16),
				Convert.ToByte(green, 16),
				Convert.ToByte(blue, 16));
		}

		public static Size ResizeToFit(Size original, Size parent)
		{
			float ratio = Math.Min(
				(float)parent.Width / original.Width,
				(float)parent.Height / original.Height);

			return new Size(
				(int)Math.Truncate(original.Width * ratio),
				(int)Math.Truncate(original.Height * ratio));
		}

		public static Size Inflate(this Size size, int x, int y)
		{
			return new Size(size.Width + x, size.Height + y);
		}

		public static void FillCircle(this Graphics graphics, Brush brush, Point point, int radius)
		{
			graphics.FillEllipse(brush, point.X - radius, point.Y - radius, radius * 2, radius * 2);
		}

		public static void FillCircle(this Graphics graphics, Brush outerBrush, Brush innerBrush, Point point, int outerRadius, int innerRadius)
		{
			FillCircle(graphics, outerBrush, point, outerRadius);
			FillCircle(graphics, innerBrush, point, innerRadius);
		}

		public static Color GetAverageColor(FastBitmap image, IEnumerable<Point> polygon)
		{
			if ((polygon.Count() < 3) || (image == null))
				return default(Color);

			long totalR = 0;
			long totalG = 0;
			long totalB = 0;
			int pixelCount = 0;

			// Get the min and max bounds for this polygon
			Point min = new Point(int.MaxValue, int.MaxValue);
			Point max = new Point(int.MinValue, int.MinValue);

			foreach (var point in polygon)
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

			var path = new System.Drawing.Drawing2D.GraphicsPath();
			path.AddPolygon(polygon.ToArray());
			path.CloseAllFigures();

			var region = new System.Drawing.Region(path);

			var bitmap = image.GetBitmap();
			var bitmapBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			using (var graphics = Graphics.FromImage(bitmap))
			{
				for (var y = min.Y; y <= max.Y; y++)
				{
					for (var x = min.X; x <= max.X; x++)
					{
						if (region.IsVisible(x, y, graphics) && bitmapBounds.Contains(x, y))
						{
							var color = image.GetPixel(x, y);
							totalR += color.R;
							totalG += color.G;
							totalB += color.B;
							pixelCount++;
						}
					}
				}
			}

			if (pixelCount == 0)
				return default(Color);

			int averageR = (byte)Math.Min(255, totalR / pixelCount);
			int averageG = (byte)Math.Min(255, totalG / pixelCount);
			int averageB = (byte)Math.Min(255, totalB / pixelCount);

			return Color.FromArgb(averageR, averageG, averageB);
		}

		public static Color BlendColors(Color[] colors)
		{
			var components = new int[] { 0, 0, 0, 0 }; 

			foreach (var color in colors)
			{
				components[0] += color.A;
				components[1] += color.R;
				components[2] += color.G;
				components[3] += color.B;
			}

			for (var index = 0; index < 4; index++)
 				components[index] = (int)Math.Min(255, components[index] / (float)colors.Length);

			return Color.FromArgb(components[0], components[1], components[2], components[3]);
		}
	}
}
