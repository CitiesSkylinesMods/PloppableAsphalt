using ColossalFramework.IO;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace PloppableAsphalt
{
	public class Configuration
	{
		[XmlIgnore]
		private static readonly string configurationPath = Path.Combine(DataLocation.localApplicationData, "PloppableAsphalt.xml");

		public Color AsphaltColor = new Color(128f, 128f, 128f, 1f);

		public bool AutoDeployed = false;

		public void OnPreSerialize()
		{
		}

		public void OnPostDeserialize()
		{
		}

		public static void SaveConfiguration()
		{
			string path = configurationPath;
			Configuration settings = PloppableAsphaltMod.Settings;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
			using (StreamWriter textWriter = new StreamWriter(path))
			{
				settings.OnPreSerialize();
				xmlSerializer.Serialize(textWriter, settings);
			}
		}

		public static Configuration LoadConfiguration()
		{
			string text = configurationPath;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
			try
			{
				using (StreamReader textReader = new StreamReader(text))
				{
					return xmlSerializer.Deserialize(textReader) as Configuration;
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"[Ploppable Asphalt]: Error Parsing {text}: {ex.Message.ToString()}");
				return null;
			}
		}
	}
}
