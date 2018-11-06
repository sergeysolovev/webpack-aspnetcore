using Microsoft.AspNetCore.Http;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Options for webpack dev server
    /// </summary>
    public class DevServerOptions
    {
        public DevServerOptions()
        {
            ManifestFileName = "manifest.json";
        }

        /// <summary>
        /// The asset manifest file name
        /// Default: manifest.json
        /// </summary>
        public string ManifestFileName { get; set; }

        /// <summary>
        /// Public path
        /// </summary>
        public PathString PublicPath { get; set; }

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
        /// Toggle server certificate validation if <see cref="Scheme"/> is 'https'
        /// </summary>
        public bool ValidateServerCert { get; set; } = true;
    }
}
