namespace Webpack.AspNetCore.Internal
{
    /// <summary>
    /// Determines the way to serve the assets and map their paths
    /// </summary>
    internal enum AssetServingMethod
    {
        /// <summary>
        /// Static
        /// </summary>
        Static = 0,

        /// <summary>
        /// Dev Server
        /// </summary>
        DevServer = 1
    }
}
