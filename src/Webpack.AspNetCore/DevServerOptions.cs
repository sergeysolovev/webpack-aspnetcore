using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Options for webpack dev server
    /// </summary>
    public class DevServerOptions
    {
        public DevServerOptions()
        {
            ManifestPath = new PathString("/manifest.json");
        }

        /// <summary>
        /// The asset manifest path withing the dev server's public path
        /// Default: /manifest.json
        /// </summary>
        public PathString ManifestPath { get; set; }

        /// <summary>
        /// Host. Default: 127.0.0.1
        /// <remarks>
        /// In some environments using "localhost" as can increase connection time
        /// and/or ns resolve time, that's why "127.0.0.1" is used as a default host value
        /// </remarks>
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port. Default: 8080
        /// </summary>
        public int Port { get; set; } = 8080;

        /// <summary>
        /// Scheme. Default: http
        /// </summary>
        public string Scheme { get; set; } = "http";

        /// <summary>
        /// Public path
        /// </summary>
        public PathString PublicPath { get; set; }
    }
}
