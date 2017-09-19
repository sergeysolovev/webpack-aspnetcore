using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace Webpack.AspNetCore
{
    public class WepbackAssetUrlRepository
    {
        private readonly WebpackContext context;
        private readonly WebpackManifestStorage storage;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<WepbackAssetUrlRepository> logger;

        public WepbackAssetUrlRepository(
            WebpackContext context,
            WebpackManifestStorage storage,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WepbackAssetUrlRepository> logger)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));

            this.storage = storage ??
                throw new ArgumentNullException(nameof(storage));

            this.httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            this.logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public string Get(string manifestAssetKey)
        {
            var manifestAssetUrl = storage.Get(manifestAssetKey) ?? storage.Get(withForwardSlash());

            if (string.IsNullOrEmpty(manifestAssetUrl))
            {
                return null;
            }

            var options = context.Options;
            var pathBase = makePath(httpContextAccessor.HttpContext.Request.PathBase);
            var assetPath = makePath(manifestAssetUrl);
            var publicPath = (options.UseStaticFiles || options.KeepOriginalAssetUrls) ?
                pathBase :
                pathBase.Add(context.ManifestPathBase);

            var assetUrl = publicPath.Add(assetPath).Value;

            logger.LogInformation(
                $"Mapped webpack asset key '{manifestAssetKey}' to url '{assetUrl}'. " +
                $"Public path: '{publicPath}'. Asset path base: '{context.ManifestPathBase}'"
            );

            return assetUrl;

            string withForwardSlash() => manifestAssetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }
    }
}
