using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Polygen.UI
{
	public abstract class Animation
	{
		public Animation(ImageCanvas canvas)
		{
			Canvas = canvas;
		}

		public ImageCanvas Canvas { get; protected set; }

		public abstract void Draw(Graphics graphics, Rectangle displayRect);

		public bool Finished { get; protected set; }
	}
}
