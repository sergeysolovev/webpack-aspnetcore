using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore
{
    public class WebpackOptions
    {
        public WebpackOptions()
        {
            ManifestPath = new PathString("/asset-manifest.json");
            UseStaticFileMiddleware = true;
            StaticOptions = new StaticOptions();
            DevServerOptions = new DevServerOptions();
        }

        /// <summary>
        /// The asset manifest path  
        /// within the application's web root path for static assets or
        /// withing the dev server's public path for dev server assets
        /// Default: /asset-manifest.json
        /// </summary>
        public PathString ManifestPath { get; set; }

        /// <summary>
        /// Determines whether to use <see cref="Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware"/>
        /// to serve static assets from the root of <see cref="ManifestPath"/>.
        /// Default: <c>true</c>
        /// </summary>
        public bool UseStaticFileMiddleware { get; set; }

        /// <summary>
        /// Static options
        /// </summary>
        internal StaticOptions StaticOptions { get; set; }

        /// <summary>
        /// Dev server options
        /// </summary>
        internal DevServerOptions DevServerOptions { get; set; }

        /// <summary>
        /// Whether to serve the assets and map their paths in
        /// static or dev server mode. Default: <see cref="AssetServingMethod.Static"/>
        /// </summary>
        internal AssetServingMethod AssetServingMethod { get; set; }

        /// <summary>
        /// Configures static asset options
        /// </summary>
        /// <param name="setupAction"></param>
        public void ConfigureStatic(Action<StaticOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            setupAction.Invoke(StaticOptions);
        }

        /// <summary>
        /// Configure dev server asset options
        /// </summary>
        /// <param name="setupAction"></param>
        public void ConfigureDevServer(Action<DevServerOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            setupAction.Invoke(DevServerOptions);
        }
    }
}
