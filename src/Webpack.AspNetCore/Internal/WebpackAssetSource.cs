namespace Webpack.AspNetCore.Internal
{
    /// <summary>
    /// Determines where webpack assets come from
    /// </summary>
    internal enum WebpackAssetSource
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
