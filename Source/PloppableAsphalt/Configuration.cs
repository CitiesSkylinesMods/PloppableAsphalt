namespace PloppableAsphalt
{
    using ColossalFramework.IO;
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using UnityEngine;

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
			var settings = PloppableAsphaltMod.Settings;
			var xmlSerializer = new XmlSerializer(typeof(Configuration));
            using var textWriter = new StreamWriter(path);
            settings.OnPreSerialize();
            xmlSerializer.Serialize(textWriter, settings);
        }

		public static Configuration LoadConfiguration()
		{
			string text = configurationPath;
			var xmlSerializer = new XmlSerializer(typeof(Configuration));
			try
			{
                using var textReader = new StreamReader(text);
                return xmlSerializer.Deserialize(textReader) as Configuration;
            }
			catch (Exception ex)
			{
				Debug.Log($"[Ploppable Asphalt]: Error Parsing {text}: {ex.Message}");
				return null;
			}
		}
	}
}
