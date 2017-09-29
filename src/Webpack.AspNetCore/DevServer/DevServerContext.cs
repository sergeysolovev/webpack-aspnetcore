using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Webpack.AspNetCore.DevServer
{
    internal class DevServerContext
    {
        private readonly DevServerOptions options;
        private readonly Uri manifestUri;
        private readonly Uri devServerUri;
        private readonly string devServerHost;

        public DevServerContext(IOptions<DevServerOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            var options = optionsAccessor.Value;

            var devServer = new UriBuilder
            {
                Host = options.Host,
                Port = options.Port,
                Scheme = options.Scheme,
                Path = options.PublicPath
            };

            var manifestUri = new Uri(devServer.Uri, options.ManifestFileName);
            var devServerHost = $"{options.Host}:{options.Port}";

            this.options = options;
            this.manifestUri = manifestUri;
            this.devServerUri = devServer.Uri;
            this.devServerHost = devServerHost;
        }

        public DevServerOptions Options => options;
        public Uri ManifestUri => manifestUri;
        public Uri DevServerUri => devServerUri;
        public string DevServerHost => devServerHost;
    }
}
