using System;
using Microsoft.AspNetCore.Builder;
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

        public bool UseDevServer { get; set; }

        /// <summary>
        /// Default - 127.0.0.1
        /// </summary>
        public string DevServerHost { get; set; } = "127.0.0.1";

        /// <summary>
        /// Default - 8080
        /// </summary>
        public int DevServerPort { get; set; } = 8080;

        /// <summary>
        /// Default - http
        /// </summary>
        public string DevServerScheme { get; set; } = "http";

        /// <summary>
        /// Default - /
        /// </summary>
        public PathString DevServerPublicPath { get; set; } = PathString.Empty;

        /// <summary>
        /// Useful for reverse proxy url rewriting
        /// Only relevant if UseStaticFiles is set to false, otherwise it's ignored
        /// </summary>
        public bool KeepOriginalAssetUrls { get; set; }

        internal StaticFileLimitedOptions StaticFileOptions { get; set; }

        public void UseStaticFiles(Action<StaticFileLimitedOptions> configureOptions = null)
        {
            StaticFileOptions = new StaticFileLimitedOptions();
            configureOptions?.Invoke(StaticFileOptions);
        }
    }
}
