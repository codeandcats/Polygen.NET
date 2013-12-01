using System;
using System.Drawing;
using System.Xml.Serialization;
using Polygen;

namespace Polygen.Schema
{
	[Serializable]
	public struct Colour : IXmlSerializable, IEquatable<Colour>, IEquatable<Color>
	{
		public Colour(Color color)
		{
			this.color = color;
		}

		private Color color;
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		public static implicit operator Color(Colour colour)
		{
			return colour.Color;
		}

		public static implicit operator Colour(Color color)
		{
			return new Colour(color);
		}

		public byte A { get { return Color.A; } }
		public byte R { get { return Color.R; } }
		public byte G { get { return Color.G; } }
		public byte B { get { return Color.B; } }

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			string value = reader.ReadContentAsString();
			reader.ReadEndElement();

			Color = GraphicsUtils.ColorFromHexString(value);
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteValue(GraphicsUtils.ColorToHexString(Color));
		}

		public bool Equals(Colour other)
		{
			return (
				(A == other.A) &&
				(R == other.R) &&
				(G == other.G) &&
				(B == other.B)
			);

		}

		public bool Equals(Color other)
		{
			return (
				(A == other.A) &&
				(R == other.R) &&
				(G == other.G) &&
				(B == other.B)
			);
		}

		public override int GetHashCode()
		{
			return color.GetHashCode();
		}
	}
}
