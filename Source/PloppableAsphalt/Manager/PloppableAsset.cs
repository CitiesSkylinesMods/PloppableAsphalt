namespace PloppableAsphalt.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Manages individual Ploppable Asphalt props/decals.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore")]
    public class PloppableAsset
    {
        private const float EXTENDED_RENDER_DISTANCE = 18000f;

        private const string PROP_TEXTURE_NAME  = "_APRMap";
        private const string DECAL_TEXTURE_NAME = "_MainTex";

        private static readonly Dictionary<string, PloppableType> MeshNameToPloppableType =
            new Dictionary<string, PloppableType>
        {
            { "ploppableasphalt-prop" , PloppableType.AsphaltProp  },
            { "ploppableasphalt-decal", PloppableType.AsphaltDecal },
            { "ploppablecliffgrass"   , PloppableType.CliffGrass   },
            { "ploppablegravel"       , PloppableType.Gravel       },
        };

        // Shader cache - applied to prop instances
        private static Shader roadBridgeShader;  // asphalt
        private static Shader roadShader;        // cliff/grass
        private static Shader trainBridgeShader; // gravel

        // Upwards diffuse cache - applied to decal instances
        private static Texture upwardDiffuse;

        private readonly PropInfo prop;

        private readonly BuildingInfo building;

        private readonly PloppableType ploppableType;

        private readonly Texture mainTexture;

        private readonly Texture lodTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="PloppableAsset"/> class
        /// for a prop or decal asset.
        /// </summary>
        /// <param name="propInfo">The <see cref="PropInfo"/> for the ploppable prop or decal.</param>
        /// <param name="meshName">The <c>m_mesh.name</c> from the prop or decal.</param>
        public PloppableAsset(PropInfo propInfo, string meshName)
        {
            if (propInfo is null)
                throw new ArgumentNullException(nameof(propInfo), "propInfo must not be null.");

            if (string.IsNullOrEmpty(meshName))
                throw new ArgumentNullException(nameof(meshName), "meshName must not be null or empty.");

            if (!MeshNameToPloppableType.TryGetValue(meshName, out var plopType))
                throw new ArgumentOutOfRangeException(nameof(meshName), "meshName not recognised as a PloppableType.");

            prop = propInfo;

            ploppableType = plopType;

            IsDecal = plopType == PloppableType.AsphaltDecal;

            IsAsphalt = IsDecal || plopType == PloppableType.AsphaltProp;

            mainTexture = IsDecal ? upwardDiffuse : prop.m_material.GetTexture("_ACIMap");
            lodTexture  = IsDecal ? upwardDiffuse : prop.m_lodMaterial.GetTexture("_ACIMap");

            Refresh();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PloppableAsset"/> class
        /// for a building asset which contains a ploppable decal/prop.
        /// </summary>
        /// <param name="buildingInfo">The <see cref="BuildingInfo"/> for the building.</param>
        public PloppableAsset(BuildingInfo buildingInfo)
        {
            if (buildingInfo is null)
                throw new ArgumentNullException(nameof(buildingInfo), "buildingInfo must not be null.");

            building = buildingInfo;

            IsBuilding = true;
        }

        /// <summary>
        /// Gets a value indicating whether the asset is a prop/decal with asphalt texture.
        /// </summary>
        public bool IsAsphalt { get; }

        /// <summary>
        /// Gets a value indicating whether the asset is a decal.
        /// </summary>
        private bool IsDecal { get; }

        /// <summary>
        /// Gets a value indicating whether the asset is a building.
        /// </summary>
        private bool IsBuilding { get; } // true if building asset

        /// <summary>
        /// Gets relevant shader depending on <see cref="ploppableType"/>.
        /// </summary>
        /// <remarks>Not applicable to decals.</remarks>
        private Shader ReplacementPropShader => ploppableType switch
        {
            PloppableType.AsphaltProp => roadBridgeShader,
            PloppableType.CliffGrass  => roadShader,
            PloppableType.Gravel      => trainBridgeShader,
            _ => throw new IndexOutOfRangeException("This ploppableType does not have a replacement shader."),
        };

        /// <summary>
        /// Cache shaders and textures.
        /// </summary>
        /// <remarks>
        /// This must be invoked before instantiating
        /// any <see cref="PloppableAsset"/> instances.
        /// </remarks>
        public static void InitialiseCache()
        {
            CacheShaders();
            CacheUpwardDiffuse();
        }

        /// <summary>
        /// Dispose previously cached shaders/diffuse.
        /// </summary>
        public static void ClearCache()
        {
            roadBridgeShader  = null;
            roadShader        = null;
            trainBridgeShader = null;
            upwardDiffuse     = null;
        }

        /// <summary>
        /// Sets the color of Asphalt props/decals.
        /// </summary>
        /// <remarks>
        /// RGB components should be in range 0..1 (where 1 represents 255).
        /// </remarks>
        /// <param name="color">The color to apply.</param>
        public void ApplyColor(Color color)
        {
            if (IsBuilding)
                return;

            prop.m_color0 = color;
            prop.m_color1 = color;
            prop.m_color2 = color;
            prop.m_color3 = color;
        }

        /// <summary>
        /// Refreshes the asset, setting where applicable:
        /// <list type="bullet">
        /// <item>Shader</item>
        /// <item>Textures</item>
        /// <item>Render Distances</item>
        /// <item>Scaling</item>
        /// </list>
        /// </summary>
        public void Refresh()
        {
            ApplyShader();
            ApplyTextures();
            ApplyRenderDistances();
            ApplyScaling();
        }

        /// <summary>
        /// Set render distances based on asset type.
        /// </summary>
        public void ApplyRenderDistances()
        {
            if (IsBuilding)
            {
                building.m_maxPropDistance =
                    Mathf.Max(building.m_maxPropDistance, EXTENDED_RENDER_DISTANCE);

                return;
            }

            prop.m_lodRenderDistance = IsDecal
                ? 200f
                : Mathf.Max(prop.m_lodRenderDistance, EXTENDED_RENDER_DISTANCE);

            if (!IsDecal)
            {
                prop.m_lodMaterialCombined = null;
                prop.m_maxRenderDistance =
                    Mathf.Max(prop.m_maxRenderDistance, EXTENDED_RENDER_DISTANCE);
            }
        }

        /// <summary>
        /// Used for non-decal props.
        /// </summary>
        private static void CacheShaders()
        {
            roadBridgeShader  = Shader.Find("Custom/Net/RoadBridge");  // asphalt
            roadShader        = Shader.Find("Custom/Net/Road");        // cliff/gras
            trainBridgeShader = Shader.Find("Custom/Net/TrainBridge"); // gravel
        }

        /// <summary>
        /// Used for asphalt decals.
        /// </summary>
        private static void CacheUpwardDiffuse() =>
            upwardDiffuse = UnityEngine.Object.FindObjectOfType<NetProperties>().m_upwardDiffuse;

        /// <summary>
        /// Set shader based on asset type.
        /// </summary>
        private void ApplyShader()
        {
            if (IsDecal || IsBuilding)
                return;

            if (!(prop.m_material is null))
                prop.m_material.shader = ReplacementPropShader;

            if (!(prop.m_lodMaterial is null))
                prop.m_lodMaterial.shader = ReplacementPropShader;
        }

        /// <summary>
        /// Set textures based on asset type.
        /// </summary>
        private void ApplyTextures()
        {
            if (IsBuilding)
                return;

            string name = IsDecal ? DECAL_TEXTURE_NAME : PROP_TEXTURE_NAME;

            prop.m_material.SetTexture(name, mainTexture);
            prop.m_lodMaterial.SetTexture(name, lodTexture);
        }

        /// <summary>
        /// Set scaling based on asset type.
        /// </summary>
        private void ApplyScaling()
        {
            if (IsDecal || IsBuilding)
                return;

            // todo: work out wtf is happening here
            // todo: also see if map theme size can be introduced
            // eg. pavement texture is often scaled

            prop.m_generatedInfo.m_size.z *= 2.174f; // no idea what this magic number is

            prop.m_generatedInfo.m_size.y = Mathf.Min(prop.m_generatedInfo.m_size.y, 16f);
            prop.m_generatedInfo.m_size.x *= 0.4f;
            prop.m_generatedInfo.m_size.z *= 0.4f;
        }
    }
}
