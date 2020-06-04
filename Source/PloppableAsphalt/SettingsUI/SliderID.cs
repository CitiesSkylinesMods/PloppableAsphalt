namespace PloppableAsphalt.SettingsUI
{
    /// <summary>
    /// IDs for the RGB sliders.
    /// </summary>
    public enum SliderID
    {
        /// <summary>
        /// Invalid slider.
        /// </summary>
        None = 0,

        /// <summary>
        /// The red slider.
        /// </summary>
        Red = 1,

        /// <summary>
        /// The green slider.
        /// </summary>
        Green = 1 << 1,

        /// <summary>
        /// The blue slider.
        /// </summary>
        Blue = 1 << 2,
    }
}
