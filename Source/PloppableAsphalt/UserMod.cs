namespace PloppableAsphalt
{
    using System.Diagnostics.CodeAnalysis;
    using ICities;
    using JetBrains.Annotations;
    using PloppableAsphalt.Manager;
    using UnityEngine;

    // Remember to check the folders in Solution > Infrastructure > Directory.Build.props before building --->

    /// <summary>
    /// Main interface to the game.
    /// </summary>
    public class UserMod : IUserMod
    {
        /// <summary>
        /// Gets a value indicating whether the mod is currently enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets name of the mod, which is shown in content manager and options.
        /// </summary>
        [UsedImplicitly]
        public string Name =>
            "## Ploppable Asphalt";

        /// <summary>
        /// Gets description of the mod, which is show in content manager.
        /// </summary>
        [UsedImplicitly]
        public string Description =>
            "Allows using road shaders on props for ploppable asphalt, pavement, cliff, grass, gravel surfaces.";

        /// <summary>
        /// <para>Invoked when user changes RGB sliders.</para>
        /// <para>Updates the Ploppable Asphalt color.</para>
        /// </summary>
        /// <param name="newColor">The new color selected by the user.</param>
        public static void SetAsphaltColor(Color newColor)
        {
            PloppableAsphaltManager.SetAsphaltColor(newColor);
            // todo: persist
        }

        /// <summary>
        /// Invoked by the game when the mod is enabled.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Game API.")]
        public void OnEnabled() => IsEnabled = true;

        /// <summary>
        /// Invoked by the game when the mod is disabled.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Game API.")]
        public void OnDisabled() => IsEnabled = false;

        /// <summary>
        /// Renders settings UI.
        /// </summary>
        /// <param name="helper">The settings UI helper.</param>
        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Game API.")]
        public void OnSettingsUI(UIHelperBase helper)
        {
            if (helper is null)
                return;

            var uIHelperBase = helper.AddGroup("\t\t\t\t\t\t     RGB Values");
            // uIHelperBase.AddSpace(40);

            var sliders = new SettingsUI.RGBSliders(helper, SetAsphaltColor);

            // todo: persistence
        }
    }
}
