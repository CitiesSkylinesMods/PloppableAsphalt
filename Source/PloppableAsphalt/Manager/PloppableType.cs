namespace PloppableAsphalt.Manager
{
    /// <summary>
    /// Classifies the type of ploppable.
    /// </summary>
    public enum PloppableType
    {
        /// <summary>
        /// Invlaid type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Asphalt texture; prop.
        /// </summary>
        AsphaltProp = 1,

        /// <summary>
        /// Asphalt texture; decal.
        /// </summary>
        AsphaltDecal = 1 << 1,

        /// <summary>
        /// Cliff or grass texture.
        /// </summary>
        CliffGrass = 1 << 2,

        /// <summary>
        /// Gravel texture.
        /// </summary>
        Gravel = 1 << 3,
    }
}
