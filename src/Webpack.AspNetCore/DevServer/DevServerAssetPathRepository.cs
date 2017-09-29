using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Implementation of <see cref="IAssetPathRepository"/>
    /// for webpack dev server. Uses <see cref="HttpContext"/>
    /// to determine asset urls' path base.
    /// </summary>
    internal class DevServerAssetPathRepository : IAssetPathRepository
    {
        private readonly DevServerContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DevServerManifestReader manifestReader;
        private readonly ILogger<DevServerAssetPathRepository> logger;
        private IDictionary<string, string> cachedManifest;

        public DevServerAssetPathRepository(
            DevServerContext context,
            DevServerManifestReader manifestReader,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DevServerAssetPathRepository> logger)
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

        public async ValueTask<string> Get(string assetKey)
        {
            var manifest = await GetManifestAsync();

            if (manifest != null)
            {
                if (manifest.TryGetValue(assetKey, out var assetUrl) ||
                    manifest.TryGetValue(withForwardSlash(), out assetUrl))
                {
                    var assetRelativePath = makePath(assetUrl);
                    var webAppPublicPath = makePath(httpContextAccessor.HttpContext.Request.PathBase);
                    var devServerPublicPath = makePath(context.Options.PublicPath);

                    if (webAppPublicPath != devServerPublicPath)
                    {
                        throw new WebpackDevServerException(
                            $"The request's path base '{webAppPublicPath}' is different from " +
                            $"webpack dev server public path '{devServerPublicPath}'. " +
                            "They must have the same value"
                        );
                    }

                    var assetPath = webAppPublicPath.Add(assetRelativePath).Value;

                    logger.LogDebug(
                        $"Mapped webpack asset key '{assetKey}' to the path '{assetPath}'. " +
                        $"Public path: '{webAppPublicPath}'."
                    );

                    return assetPath;
                }
            }

            return null;

            string withForwardSlash() => assetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }

        /// <summary>
        /// Checks whether asset manifest contains a specified path
        /// </summary>
        /// <param name="requestPath"></param>
        /// <returns></returns>
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
