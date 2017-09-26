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
        }

        /// <summary>
        /// The asset manifest path within the application's web root path.
        /// Defaults to /asset-manifest.json
        /// </summary>
        public PathString ManifestPath { get; set; }

        /// <summary>
        /// Useful for reverse proxy url rewriting
        /// Only relevant if UseStaticFiles is set to false, otherwise it's ignored
        /// </summary>
        public bool KeepOriginalAssetUrls { get; set; }

        internal StaticFileLimitedOptions StaticFileOptions { get; set; }

        internal DevServerOptions DevServerOptions { get; set; }

        public void UseStaticFiles(Action<StaticFileLimitedOptions> configureOptions = null)
        {
            StaticFileOptions = new StaticFileLimitedOptions();
            configureOptions?.Invoke(StaticFileOptions);
        }

        public void UseDevServer(Action<DevServerOptions> configureOptions = null)
        {
            DevServerOptions = new DevServerOptions();
            configureOptions?.Invoke(DevServerOptions);
        }
    }
}
