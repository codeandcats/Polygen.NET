using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Polygen.Schema
{
	[Serializable]
	public struct SerializableColor //: IXmlSerializable
	{
		private Color color;

		public SerializableColor(Color color)
		{
			this.color = color;
		}

		public SerializableColor(string web)
		{
			this.color = ColorTranslator.FromHtml(web);
		}

		//[XmlAttribute(AttributeName="Value")]
		[XmlText]
		public string Web
		{
			get
			{
				return ColorTranslator.ToHtml(color);
			}
			set
			{
				color = ColorTranslator.FromHtml(value);
			}
		}

		/*
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			var value = reader.ReadContentAsString();
			color = ColorTranslator.FromHtml(value);
		}

		public void WriteXml(XmlWriter writer)
		{
			string value = ColorTranslator.ToHtml(color);
			writer.WriteValue(value);
		}
		*/

		public static implicit operator SerializableColor(string webColor)
		{
			return new SerializableColor(webColor);
		}

		public override bool Equals(object obj)
		{
			if (obj is SerializableColor)
				return ((SerializableColor)obj).color.Equals(color);
			else if (obj is Color)
				return ((Color)obj).Equals(color);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return color.GetHashCode();
		}

		public static implicit operator Color(SerializableColor color)
		{
			return color.color;
		}

		public static implicit operator SerializableColor(Color color)
		{
			return new SerializableColor(color);
		}

		public int A { get { return color.A; } }

		public int R { get { return color.R; } }

		public int G { get { return color.G; } }

		public int B { get { return color.B; } }
	}
}
