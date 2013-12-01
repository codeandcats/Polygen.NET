using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Polygen
{
	public static class Settings
	{
		static Settings()
		{
			RecentFileNames = new RecentItemList<string>(new StringEqualityComparer(true));
			Snapping = true;
			ShowPolygonOutlines = true;
			ImageOpacity = 100;
		}

		public static RecentItemList<string> RecentFileNames { get; set; }

		public static bool Snapping { get; set; }

		public static bool ShowPolygonOutlines { get; set; }

		private static byte imageOpacity = 100;
		public static byte ImageOpacity {
			get { return imageOpacity; }
			set { imageOpacity = value; }
		}

		private static string GetSettingsFileName()
		{
			string fileName = Assembly.GetExecutingAssembly().FullName;
			fileName = Path.Combine(Path.GetDirectoryName(fileName), "PolygenSettings.xml");
			return fileName;
		}

		private static string GetNodeInnerText(XmlNode parent, string xpath)
		{
			XmlNode child = parent.SelectSingleNode(xpath);
			if (child == null)
				return "";
			else
				return child.InnerText;
		}

		public static void Load()
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(GetSettingsFileName());

			// Load recent file names
			RecentFileNames.Clear();

			var items = new List<string>();
			foreach (XmlNode recentFileNode in xml.SelectNodes("/settings/recent-files/recent-file"))
				items.Add(recentFileNode.InnerText);

			for (var index = items.Count - 1; index >= 0; index--)
				RecentFileNames.MostRecent = items[index];

			// Load Snapping
			var snappingStr = GetNodeInnerText(xml, "/settings/snapping");
			bool snapping;
			if (!bool.TryParse(snappingStr, out snapping))
				Snapping = true;
			else
				Snapping = snapping;

			// Load ShowPolygonOutlines
			var showPolygonOutlinesStr = GetNodeInnerText(xml, "/settings/show-polygon-outlines");
			bool showPolygonOutlines;
			if (!bool.TryParse(showPolygonOutlinesStr, out showPolygonOutlines))
				ShowPolygonOutlines = true;
			else
				ShowPolygonOutlines = showPolygonOutlines;

			// Load ImageOpacity
			var imageOpacityStr = GetNodeInnerText(xml, "/settings/image-opacity");
			byte imageOpacity;
			if (!byte.TryParse(imageOpacityStr, out imageOpacity))
				ImageOpacity = 100;
			else
				ImageOpacity = imageOpacity;
		}

		private static XmlNode CreateElement(XmlDocument xml, XmlNode parent, string name, string innerText)
		{
			XmlElement element = xml.CreateElement(name);
			element.InnerText = innerText;
			parent.AppendChild(element);
			return element;
		}

		public static void Save()
		{
			XmlDocument xml = new XmlDocument();

			var settingsNode = xml.CreateElement("settings");
			settingsNode.SetAttribute("version", "2013.3.10.1");

			xml.AppendChild(settingsNode);

			// Write recent filenames
			var recentFilesNode = xml.CreateElement("recent-files");

			for (var index = 0; index < RecentFileNames.Count; index++)
			{
				string fileName = RecentFileNames[index];

				var recentFileNode = xml.CreateElement("recent-file");
				recentFileNode.InnerText = fileName;

				recentFilesNode.AppendChild(recentFileNode);
			}

			settingsNode.AppendChild(recentFilesNode);

			CreateElement(xml, settingsNode, "snapping", Snapping.ToString());

			CreateElement(xml, settingsNode, "show-polygon-outlines", ShowPolygonOutlines.ToString());

			CreateElement(xml, settingsNode, "image-opacity", ImageOpacity.ToString());

			xml.Save(GetSettingsFileName());
		}
	}
}
