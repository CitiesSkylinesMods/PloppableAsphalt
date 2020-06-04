namespace PloppableAsphalt.Manager
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using ColossalFramework.UI;
    using JetBrains.Annotations;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    /// <summary>
    /// Manages the everything relating to ploppable asphalt props while in-game.
    /// </summary>
    public class PloppableAsphaltManager : MonoBehaviour
    {
        private static UITextureSprite desaturationTarget;

        private static PloppableAsphaltManager instance;

        private static Color currentColor = Color.magenta;

        private ICollection<PloppableAsset> assets;

        /// <summary>
        /// Applies color to asphalt props/decals.
        /// </summary>
        /// <remarks>
        /// Expects RGB component values in range 0..255.
        /// </remarks>
        /// <param name="color">The color to apply.</param>
        public static void SetAsphaltColor(Color color)
        {
            currentColor = color;

            if (instance)
            {
                var asphaltColor = new Color(color.r / 255, color.g / 255, color.b / 255, 1f);

                foreach (var asset in instance.assets)
                    if (asset.IsAsphalt)
                        asset.ApplyColor(asphaltColor);
            }
        }

        /// <summary>
        /// Initialise the manager.
        /// </summary>
        [UsedImplicitly]
        protected void Start()
        {
            var startTimer = Stopwatch.StartNew();

            PloppableAsset.InitialiseCache();

            var list = new List<PloppableAsset>(500);

            AssetScanner.CollateAssets(list);

            if (list.Count == 0)
            {
                OnDestroy();
                return;
            }

            list.TrimExcess();
            assets = list;

            instance = this;
            HookEvents(true);
            SetAsphaltColor(currentColor);

            enabled = ObtainDesaturationTarget();

            Debug.Log($"[PloppableAsphalt] Start() time: {startTimer.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// Keep opacity of <see cref="desaturationTarget"/> locked at
        /// <c>0f</c> to prevent blurring of ploppale props.
        /// </summary>
        /// <remarks>PERFORMANCE CRITICAL.</remarks>
        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Unity API.")]
        protected void LateUpdate()
        {
            if (desaturationTarget.opacity != 0f)
                desaturationTarget.opacity = 0f;
        }

        /// <summary>
        /// Destroy the manager.
        /// </summary>
        [UsedImplicitly]
        protected void OnDestroy()
        {
            HookEvents(false);
            enabled = false;
            instance = null;
            CancelInvoke();
            desaturationTarget = null;
            assets = null;
            PloppableAsset.ClearCache();
        }

        /// <summary>
        /// Obtain the <see cref="desaturationTarget"/> for the
        /// <see cref="LateUpdate()"/> loop.
        /// </summary>
        /// <returns>Returns <c>true</c> if successful, otherwise <c>false</c>.</returns>
        private static bool ObtainDesaturationTarget()
        {
            var desaturationControl = FindObjectOfType<DesaturationControl>();

            if (desaturationControl is null)
            {
                Debug.Log("[PloppableAsphalt] Failed to obtain desaturationControl; ploppables may look blurred.");
                return false;
            }

            desaturationTarget = desaturationControl.GetComponent<UITextureSprite>();

            if (desaturationTarget is null)
            {
                Debug.Log("[PloppableAsphalt] Failed to obtain desaturationTarget; ploppables may look blurred.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Refresh ploppable assets when game options panel closes.
        /// </summary>
        /// <param name="component">Ignored.</param>
        /// <param name="visible">The visibility of the options panel.</param>
        private static void OnOptionsVisibilityChanged(UIComponent component, bool visible)
        {
            if (!visible)
                foreach (var asset in instance.assets)
                    asset.Refresh();
        }

        /// <summary>
        /// Reapply render distance changes when level of detail is changed.
        /// </summary>
        /// <param name="component">Ignored.</param>
        /// <param name="index">Ignored.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:Element documentation should not be copied and pasted")]
        private static void OnLevelOfDetailChanged(UIComponent component, int index)
        {
            foreach (var asset in instance.assets)
                asset.ApplyRenderDistances();
        }

        /// <summary>
        /// Hook events so that ploppable assets can be refreshed when game options change.
        /// </summary>
        /// <param name="enable">Hooks when <c>true</c>, unhooks when <c>false</c>.</param>
        [SuppressMessage("Naming", "AV1738:Event handlers should be named according to the pattern '(InstanceName)On(EventName)'", Justification = "Overkill.")]
        private static void HookEvents(bool enable)
        {
            var optionsPanel = UIView.library.Get<UIPanel>("OptionsPanel");
            var levelOfDetail = UIView.GetAView().FindUIComponent<UIDropDown>("LevelOfDetail");

            if (enable)
            {
                optionsPanel.eventVisibilityChanged += OnOptionsVisibilityChanged;
                levelOfDetail.eventSelectedIndexChanged += OnLevelOfDetailChanged;
            }
            else
            {
                optionsPanel.eventVisibilityChanged -= OnOptionsVisibilityChanged;
                levelOfDetail.eventSelectedIndexChanged -= OnLevelOfDetailChanged;
            }
        }
    }
}
