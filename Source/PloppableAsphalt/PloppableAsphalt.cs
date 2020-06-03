using UnityEngine;

namespace PloppableAsphalt
{
	public class PloppableAsphalt
	{
		public static bool Loaded;

		private static Shader shader;

		private static Shader shader2;

		private static Shader shader3;

		internal static void SetRenderProperties(PropInfo prefab)
		{
			prefab.m_lodRenderDistance = ((prefab.m_mesh.name == "ploppableasphalt-prop") ? 200 : 18000);
			prefab.m_maxRenderDistance = ((prefab.m_mesh.name == "ploppableasphalt-decal") ? prefab.m_maxRenderDistance : 18000f);
			prefab.m_lodRenderDistance = ((prefab.m_mesh.name == "ploppablecliffgrass") ? 200 : 18000);
			prefab.m_lodRenderDistance = ((prefab.m_mesh.name == "ploppablegravel") ? 200 : 18000);
		}

		internal static void SetRenderProperties(BuildingInfo prefab)
		{
			prefab.m_maxPropDistance = 18000f;
		}

		internal static void SetRenderPropertiesAll()
		{
			for (uint num = 0u; num < PrefabCollection<PropInfo>.LoadedCount(); num++)
			{
				PropInfo loaded = PrefabCollection<PropInfo>.GetLoaded(num);
				if (!(loaded == null) && (loaded.m_mesh.name == "ploppableasphalt-prop" || loaded.m_mesh.name == "ploppableasphalt-decal" || loaded.m_mesh.name == "ploppablecliffgrass" || loaded.m_mesh.name == "ploppablegravel"))
				{
					SetRenderProperties(loaded);
				}
			}
			for (uint num2 = 0u; num2 < PrefabCollection<BuildingInfo>.LoadedCount(); num2++)
			{
				BuildingInfo loaded2 = PrefabCollection<BuildingInfo>.GetLoaded(num2);
				if (loaded2 == null || loaded2.m_props == null || loaded2.m_props.Length == 0)
				{
					continue;
				}
				for (uint num3 = 0u; num3 < loaded2.m_props.Length; num3++)
				{
					if (!(loaded2.m_props[num3].m_finalProp == null) && !(loaded2.m_props[num3].m_finalProp.m_mesh == null) && (loaded2.m_props[num3].m_finalProp.m_mesh.name == "ploppableasphalt-prop" || loaded2.m_props[num3].m_finalProp.m_mesh.name == "ploppableasphalt-decal" || loaded2.m_props[num3].m_finalProp.m_mesh.name == "ploppablecliffgrass" || loaded2.m_props[num3].m_finalProp.m_mesh.name == "ploppablegravel"))
					{
						SetRenderProperties(loaded2);
					}
				}
			}
		}

		internal static void ApplyProperties()
		{
			shader = Shader.Find("Custom/Net/RoadBridge");
			shader2 = Shader.Find("Custom/Net/Road");
			shader3 = Shader.Find("Custom/Net/TrainBridge");
			for (uint num = 0u; num < PrefabCollection<PropInfo>.LoadedCount(); num++)
			{
				PropInfo loaded = PrefabCollection<PropInfo>.GetLoaded(num);
				if (loaded == null)
				{
					continue;
				}
				if (loaded.m_mesh.name == "ploppableasphalt-prop" || loaded.m_mesh.name == "ploppablecliffgrass" || loaded.m_mesh.name == "ploppablegravel")
				{
					Texture texture = new Texture2D(1, 1);
					Texture texture2 = new Texture2D(1, 1);
					texture = (loaded.m_material.GetTexture("_ACIMap") as Texture2D);
					texture2 = (loaded.m_lodMaterial.GetTexture("_ACIMap") as Texture2D);
					if (loaded.m_mesh.name == "ploppableasphalt-prop")
					{
						if (loaded.m_material != null)
						{
							loaded.m_material.shader = shader;
						}
						if (loaded.m_lodMaterial != null)
						{
							loaded.m_lodMaterial.shader = shader;
						}
					}
					else if (loaded.m_mesh.name == "ploppablecliffgrass")
					{
						if (loaded.m_material != null)
						{
							loaded.m_material.shader = shader2;
						}
						if (loaded.m_lodMaterial != null)
						{
							loaded.m_lodMaterial.shader = shader2;
						}
					}
					else if (loaded.m_mesh.name == "ploppablegravel")
					{
						if (loaded.m_material != null)
						{
							loaded.m_material.shader = shader3;
						}
						if (loaded.m_lodMaterial != null)
						{
							loaded.m_lodMaterial.shader = shader3;
						}
					}
					loaded.m_material.SetTexture("_APRMap", texture);
					loaded.m_lodMaterial.SetTexture("_APRMap", texture2);
					SetRenderProperties(loaded);
					loaded.m_lodMaterialCombined = null;
					loaded.m_generatedInfo.m_size.z = loaded.m_generatedInfo.m_size.z * 2.174f;
					if (loaded.m_generatedInfo.m_size.y < 16f)
					{
						loaded.m_generatedInfo.m_size.y = 16f;
					}
					loaded.m_generatedInfo.m_size.x = loaded.m_generatedInfo.m_size.x * 0.4f;
					loaded.m_generatedInfo.m_size.z = loaded.m_generatedInfo.m_size.z * 0.4f;
				}
				else if (loaded.m_mesh.name == "ploppableasphalt-decal")
				{
					loaded.m_material.SetTexture("_MainTex", Object.FindObjectOfType<NetProperties>().m_upwardDiffuse);
					loaded.m_lodMaterial.SetTexture("_MainTex", Object.FindObjectOfType<NetProperties>().m_upwardDiffuse);
					SetRenderProperties(loaded);
				}
				ApplyColors();
			}
		}

		internal static void ApplyColors()
		{
			for (uint num = 0u; num < PrefabCollection<PropInfo>.LoadedCount(); num++)
			{
				Color asphaltColor = PloppableAsphaltMod.Settings.AsphaltColor;
				PropInfo loaded = PrefabCollection<PropInfo>.GetLoaded(num);
				Color color = new Color(asphaltColor.r / 255f, asphaltColor.g / 255f, asphaltColor.b / 255f);
				if (!(loaded == null) && (loaded.m_mesh.name == "ploppableasphalt-prop" || loaded.m_mesh.name == "ploppableasphalt-decal" || loaded.m_mesh.name == "ploppablegravel" || loaded.m_mesh.name == "ploppablecliffgrass"))
				{
					loaded.m_color0 = color;
					loaded.m_color1 = color;
					loaded.m_color2 = color;
					loaded.m_color3 = color;
				}
			}
			Configuration.SaveConfiguration();
			SetRenderPropertiesAll();
		}
	}
}
