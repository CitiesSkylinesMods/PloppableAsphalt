namespace PloppableAsphalt
{
    using ICities;
    using JetBrains.Annotations;
    using PloppableAsphalt.Manager;
    using UnityEngine;

    /// <summary>
    /// Loading extension for PloppableAsphalt mod.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        private GameObject gameObject;

        /// <summary>
        /// If applicable <paramref name="mode"/>, creates a <see cref="GameObject"/> for the <see cref="PloppableAsphaltManager"/>.
        /// </summary>
        /// <param name="mode">The <see cref="LoadMode"/> applicable to this loading extension.</param>
        [UsedImplicitly]
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (UserMod.IsEnabled && IsApplicable(mode))
            {
                Debug.Log($"[AdditiveShader] Initialising for LoadMode: {mode}");
                gameObject = new GameObject();
                gameObject.AddComponent<PloppableAsphaltManager>();
            }
        }

        /// <summary>
        /// If it exists, destroys the <see cref="PloppableAsphaltManager"/> and <see cref="GameObject"/>.
        /// </summary>
        [UsedImplicitly]
        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (!gameObject)
                return;

            Object.Destroy(gameObject);
            gameObject = null;
        }

        /// <summary>
        /// Determines whether the <paramref name="mode"/> is applicable to this mod.
        /// </summary>
        /// <param name="mode">The <see cref="LoadMode"/> applicable to this instance.</param>
        /// <returns>Returns <c>true</c> if applicable, otherwise <c>false</c>.</returns>
        private static bool IsApplicable(LoadMode mode) =>
            mode == LoadMode.NewGame ||
            mode == LoadMode.NewGameFromScenario ||
            mode == LoadMode.LoadGame;
    }
}
