using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Implementation of <see cref="IAssetUrlRepository"/>
    /// for webpack dev server. Uses <see cref="HttpContext"/>
    /// to determine asset urls' path base.
    /// </summary>
    internal class DevServerManifestRepository : IAssetUrlRepository
    {
        private readonly WebpackContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DevServerManifestReader manifestReader;
        private readonly ILogger<DevServerManifestRepository> logger;
        private IDictionary<string, string> cachedManifest;

        public DevServerManifestRepository(
            WebpackContext context,
            DevServerManifestReader manifestReader,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DevServerManifestRepository> logger)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));

            this.manifestReader = manifestReader ??
                throw new ArgumentNullException(nameof(manifestReader));

            this.httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            this.logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> HasMatchingPath(string requestPath)
        {
            var manifest = await GetManifestAsync();

            if (manifest != null)
            {
                bool matches(string assetUrl) => requestPath.Contains(assetUrl);

                // Avoid creating excessive closure with
                // Enumerable.Any, that's why foreach
                foreach (var assetUrl in manifest.Values)
                {
                    if (matches(assetUrl)) return true;
                }
            }

            return false;
        }

        public async ValueTask<string> Get(string manifestAssetKey)
        {
            var manifest = await GetManifestAsync();

            if (manifest != null)
            {
                if (manifest.TryGetValue(manifestAssetKey, out var manifestAssetUrl) ||
                    manifest.TryGetValue(withForwardSlash(), out manifestAssetUrl))
                {
                    var options = context.Options;
                    var assetPath = makePath(manifestAssetUrl);
                    var publicPath = makePath(httpContextAccessor.HttpContext.Request.PathBase);
                    var devServerPublicPath = makePath(context.Options.DevServerPublicPath);

                    if (publicPath != devServerPublicPath)
                    {
                        throw new WebpackDevServerException(
                            $"The request's path base '{publicPath}' is different from " +
                            $"webpack dev server public path '{devServerPublicPath}'. " +
                            "They must have the same value"
                        );
                    }

                    var assetUrl = publicPath.Add(assetPath).Value;
                    return assetUrl;
                }
            }

            return null;

            string withForwardSlash() => manifestAssetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }

        private async ValueTask<IDictionary<string, string>> GetManifestAsync()
        {
            if (cachedManifest == null)
            {
                cachedManifest = await manifestReader.ReadAsync();
            }

            return cachedManifest;
        }
    }
}
