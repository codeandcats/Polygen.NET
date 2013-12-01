using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Polygen.Schema;

namespace Polygen.UI
{
	public class PolygonAnimation : Animation
	{
		public PolygonAnimation(ImageCanvas canvas) :
			base(canvas)
		{
		}

		public override void Draw(Graphics graphics, Rectangle displayRect)
		{

		}

		public Polygon Polygon;

		// Fades in highlight then fades in and out until EndHighlight is called
		public void BeginHighlight()
		{
		}

		public void EndHighlight()
		{
		}
	}
}
