using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Polygen;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Polygen.Schema
{
	[Serializable]
	public class Polygon : IXmlSerializable
	{
		public Polygon()
		{
			points = new ObservableCollection<ColoredPoint>();
			points.CollectionChanged += PointsChanged;

			Color = System.Drawing.Color.LightBlue;
		}

		private ObservableCollection<ColoredPoint> points;
		public ObservableCollection<ColoredPoint> Points
		{
			get { return points; }
		}
		
		private void PointsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (pointsInUpdateCount > 0)
				return;

			if (reorderingPoints)
				return;

			ReorderPoints();
		}

		private int pointsInUpdateCount = 0;

		public void BeginPointsUpdate()
		{
			pointsInUpdateCount++;
		}

		public void EndPointsUpdate()
		{
			if (pointsInUpdateCount > 0)
				pointsInUpdateCount--;

			if (pointsInUpdateCount == 0)
				PointsChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private bool reorderingPoints = false;

		private void ReorderPoints()
		{
			if (reorderingPoints)
				return;

			reorderingPoints = true;
			try
			{
				// Points must be in clockwise / anti-clockwise order for drawing and many calculations to work
				var pointsOrdered = GeometeryUtils.GetConvexPolygonPointsSorted(points.ToArray());

				for (var index = 0; index < pointsOrdered.Length; index++)
				{
					var point = pointsOrdered[index];
					if (!points[index].Equals(point))
						points[index] = point;
				}
			}
			finally
			{
				reorderingPoints = false;
			}
		}

		private SerializableColor color;
		public SerializableColor Color
		{
			get { return color; }
			set { color = value; }
		}

		public bool ContainsPoint(Point point)
		{
			return GeometeryUtils.PolygonContainsPoint(
				Points.Select(p => p.Point).ToArray(), point);
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.ReadToDescendant("Points"))
			{
				reader.ReadStartElement();
				while (reader.IsStartElement("Point", ""))
				{
					var point = new ColoredPoint();
					
					reader.ReadStartElement();
					point.X = reader.ReadElementContentAsInt("X", "");
					point.Y = reader.ReadElementContentAsInt("Y", "");

					if ((reader.NodeType == XmlNodeType.Element) && reader.LocalName.Equals("Color"))
						point.Color = reader.ReadElementContentAsString("Color", "");

					Points.Add(point);

					reader.ReadEndElement();
				}
				reader.ReadEndElement();
			}

			if (reader.IsStartElement("Color"))
			{
				color.Web = reader.ReadElementContentAsString();
			}

			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Points");
			for (var index = 0; index < Points.Count; index++)
			{
				var point = Points[index];
				writer.WriteStartElement("Point");
				writer.WriteElementString("X", point.X.ToString());
				writer.WriteElementString("Y", point.Y.ToString());
				writer.WriteElementString("Color", point.Color.Web);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteElementString("Color", Color.Web);
		}
	}
}
