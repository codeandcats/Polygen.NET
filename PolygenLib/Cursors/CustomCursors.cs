using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Polygen.Cursors
{
	public static class CustomCursors
	{
		static CustomCursors()
		{
			Cross = CursorFromBytes(Polygen.Properties.Resources.Cross);
			Eraser = CursorFromBytes(Polygen.Properties.Resources.Eraser);
		}

		private static Cursor CursorFromBytes(byte[] bytes)
		{
			using (var stream = new System.IO.MemoryStream(bytes))
			{
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				return new Cursor(stream);
			}
		}

		public static Cursor Cross { get; private set; }

		public static Cursor Eraser { get; private set; }
	}
}
