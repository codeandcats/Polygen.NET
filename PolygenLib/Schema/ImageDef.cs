using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Polygen;
using System.Drawing;

namespace Polygen.Schema
{
	[Serializable]
	public class ImageDef
	{
		public ImageDef()
		{
			Polygons = new List<Polygon>();
		}

		public ImageDef(int width, int height)
		{
			Polygons = new List<Polygon>();

			Width = width;
			Height = height;
		}

		public int Width { get; set; }

		public int Height { get; set; }

		[NonSerialized]
		private FastBitmap baseImage = null;
		public FastBitmap GetBaseImage()
		{
			if ((baseImage == null) &&
				(baseFileName != "") &&
				File.Exists(baseFileName))
				LoadImage();

			return baseImage;
		}

		private string baseFileName = "";
		public string BaseFileName
		{
			get
			{
				return baseFileName;
			}
			set
			{
				baseFileName = value;
				LoadImage();
			}
		}

		private void LoadImage()
		{
			if (File.Exists(baseFileName))
			{
				var bmp = new Bitmap(baseFileName);
				baseImage = new FastBitmap(bmp);

				Width = bmp.Width;
				Height = bmp.Height;
			}
			else
				baseImage = null;
		}

		public List<Polygon> Polygons { get; private set; }

		public void SaveToFile(string fileName)
		{
			if (File.Exists(fileName))
				File.Delete(fileName);

			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
 			{
				var serializer = new XmlSerializer(typeof(ImageDef));
				serializer.Serialize(stream, this);
			}
		}

		public static ImageDef LoadFromFile(string fileName)
		{
			var serializer = new XmlSerializer(typeof(ImageDef));

			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var image = (ImageDef)serializer.Deserialize(stream);
				
				if (!Path.IsPathRooted(image.BaseFileName))
				{
					var currentDirectory = Environment.CurrentDirectory;

					Environment.CurrentDirectory = Path.GetDirectoryName(fileName);
					try
					{
						image.BaseFileName = Path.GetFullPath(image.BaseFileName);
					}
					finally
					{
						Environment.CurrentDirectory = currentDirectory;
					}
				}

				return image;
			}
		}

		public byte[] ToBytes()
		{
			var serializer = new BinaryFormatter();

			using (var stream = new MemoryStream())
			{
				serializer.Serialize(stream, this);

				stream.Seek(0, SeekOrigin.Begin);

				return stream.ToArray();
			}
		}

		public static ImageDef FromBytes(byte[] data)
		{
			var serializer = new BinaryFormatter();

			using (var stream = new MemoryStream(data))
			{
				stream.Seek(0, SeekOrigin.Begin);
				return (ImageDef)serializer.Deserialize(stream);
			}
		}

		public bool Shading { get; set; }

		public void UpdatePolygonColor(Polygon polygon)
		{
			polygon.Color = GraphicsUtils.GetAverageColor(baseImage, polygon.Points.Select(p => p.Point));
		}

		public void UpdatePolygonColors()
		{
			foreach (var polygon in Polygons)
			{
				UpdatePolygonColor(polygon);
			}

			UpdatePolygonPointColors();
		}

		public void UpdatePolygonPointColors()
		{
			foreach (var polygon in Polygons)
			{
				UpdatePolygonPointColors(polygon);
			}
		}

		internal void UpdatePolygonPointColors(Polygon polygon)
		{
			for (var currPointIndex = 0; currPointIndex < polygon.Points.Count; currPointIndex++)
			{
				int prevPointIndex = (currPointIndex - 1) % polygon.Points.Count;
				prevPointIndex = prevPointIndex == -1 ? polygon.Points.Count - 1 : prevPointIndex;
				int nextPointIndex = (currPointIndex + 1) % polygon.Points.Count;

				ColoredPoint prevPoint = polygon.Points[prevPointIndex];
				ColoredPoint currPoint = polygon.Points[currPointIndex];
				ColoredPoint nextPoint = polygon.Points[nextPointIndex];

				// Think of it this way, each point has 2 sides of it's polygon touching it.
				// There should be 2 polygons each sharing one of the 2 sides that connect to this point.
				// We need to calculate this point's color from the color of those 2 adjacent polygons and this polygon's color?

				var polygon1 = Polygons.FirstOrDefault(poly =>
					poly.Points.Select(p => p.Point).Contains(prevPoint.Point) &&
					poly.Points.Select(p => p.Point).Contains(currPoint.Point));

				var polygon2 = Polygons.FirstOrDefault(poly =>
					poly.Points.Select(p => p.Point).Contains(nextPoint.Point) &&
					poly.Points.Select(p => p.Point).Contains(currPoint.Point));

				var colorsToBlend = new List<Color>();
				colorsToBlend.Add(polygon.Color);

				if (polygon1 != null)
					colorsToBlend.Add(polygon1.Color);

				if (polygon2 != null)
					colorsToBlend.Add(polygon2.Color);

				currPoint.Color = GraphicsUtils.BlendColors(colorsToBlend.ToArray());

				polygon.Points[currPointIndex] = currPoint;
			}
		}
	}
}
