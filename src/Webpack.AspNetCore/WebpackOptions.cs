using Microsoft.AspNetCore.Http;

namespace Webpack.AspNetCore
{
    public class WebpackOptions
    {
        public WebpackOptions()
        {
            ManifestPath = new PathString("/asset-manifest.json");
            RequestPath = PathString.Empty;
        }

        /// <summary>
        /// The asset manifest path within the application's web root path.
        /// Defaults to /asset-manifest.json
        /// </summary>
        public PathString ManifestPath { get; set; }

        public PathString RequestPath { get; set; }

        public bool UseStaticFiles { get; set; }

        /// <summary>
        /// Useful for reverse proxy url rewriting
        /// Only relevant if UseStaticFiles is set to false, otherwise it's ignored
        /// </summary>
        public bool KeepOriginalAssetUrls { get; set; }
    }
}
