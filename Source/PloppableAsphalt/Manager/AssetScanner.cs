namespace PloppableAsphalt.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Collates list of assets associated with PloppableAsphalt mod.
    /// </summary>
    public static class AssetScanner
    {
        /// <summary>
        /// If a mesh name contains this token, it's _probably_ a Ploppable Asphalt asset.
        /// </summary>
        private const string TOKEN = "ploppable";

        /// <summary>
        /// Use to quickly confirm if a prop is a Ploppable Asphalt asset.
        /// </summary>
        private static readonly HashSet<string> ValidMeshNames = new HashSet<string>
        {
            "ploppableasphalt-prop",
            "ploppableasphalt-decal",
            "ploppablecliffgrass",
            "ploppablegravel",
        };

        /// <summary>
        /// Scans assets, adding any related to PloppableAsphalt to the list.
        /// </summary>
        /// <param name="list">The list where assets are collated.</param>
        public static void CollateAssets(List<PloppableAsset> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            CollateProps(list);
            CollateBuildings(list);
        }

        /// <summary>
        /// Scans all props (incl. decals), adding any that are ploppables
        /// to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list collating identified assets.</param>
        private static void CollateProps(List<PloppableAsset> list)
        {
            int scanned = 0;

            foreach (var prop in Resources.FindObjectsOfTypeAll<PropInfo>())
                try
                {
                    ++scanned;

                    if (prop is null || prop.m_mesh is null)
                        continue;

                    if (IsPloppable(prop.m_mesh))
                        list.Add(new PloppableAsset(prop, prop.m_mesh.name));
                }
                catch (Exception error)
                {
                    Debug.LogError($"[PloppableAsphalt] PropInfo error: {prop.name}\n{error}");
                }

            Debug.Log($"[PloppableAsphalt] PropInfo - Expected: {PrefabCollection<PropInfo>.LoadedCount()}, Scanned: {scanned}");
        }

        /// <summary>
        /// Scans all buildings, adding any that contain ploppables
        /// to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list collating identified assets.</param>
        private static void CollateBuildings(List<PloppableAsset> list)
        {
            int scanned = 0;

            foreach (var building in Resources.FindObjectsOfTypeAll<BuildingInfo>())
                try
                {
                    ++scanned;

                    if (building is null)
                        continue;

                    if (ContainsPloppable(building))
                        list.Add(new PloppableAsset(building));
                }
                catch (Exception error)
                {
                    Debug.LogError($"[PloppableAsphalt] BuildingInfo error: {building.name} \n{error}");
                }

            Debug.Log($"[PloppableAsphalt] BuildingInfo - Expected: {PrefabCollection<BuildingInfo>.LoadedCount()}, Scanned: {scanned}");
        }

        /// <summary>
        /// Test if a <see cref="BuildingInfo"/> contains a ploppable.
        /// </summary>
        /// <remarks>
        /// Assumes <paramref name="building"/>, a <see cref="UnityEngine.Object"/>,
        /// has already been verified as not <c>null</c>.
        /// </remarks>
        /// <param name="building">The <see cref="BuildingInfo"/> to test.</param>
        /// <returns>Returns <c>true</c> if it contains a ploppable, otherwise <c>false</c>.</returns>
        private static bool ContainsPloppable(BuildingInfo building)
        {
            if (building.m_props is null || building.m_props.Length == 0)
                return false;

            foreach (var prop in building.m_props)
            {
                if (prop.m_finalProp is null || prop.m_finalProp.m_mesh is null)
                    continue;

                if (IsPloppable(prop.m_finalProp.m_mesh))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// <para>Test if a <see cref="Mesh"/> is a ploppable.</para>
        /// </summary>
        /// <remarks>
        /// Assumes <paramref name="mesh"/>, a <see cref="UnityEngine.Object"/>,
        /// has already been verified as not <c>null</c>.
        /// </remarks>
        /// <param name="mesh">The <see cref="Mesh"/> to test.</param>
        /// <returns>Returns <c>true</c> if it's ploppable, otherwise <c>false</c>.</returns>
        private static bool IsPloppable(Mesh mesh) =>
            !string.IsNullOrEmpty(mesh.name) &&
            mesh.name.StartsWith(TOKEN) &&
            ValidMeshNames.Contains(mesh.name);
    }
}
