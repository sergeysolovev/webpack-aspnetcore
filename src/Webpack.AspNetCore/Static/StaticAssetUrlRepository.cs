using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore.Static
{
    internal class StaticAssetUrlRepository : IAssetUrlRepository
    {
        private readonly WebpackContext context;
        private readonly ManifestStorage storage;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<StaticAssetUrlRepository> logger;

        public StaticAssetUrlRepository(
            WebpackContext context,
            ManifestStorage storage,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StaticAssetUrlRepository> logger)
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

        public ValueTask<string> Get(string manifestAssetKey)
        {
            var manifestAssetUrl = storage.Get(manifestAssetKey) ?? storage.Get(withForwardSlash());

            if (string.IsNullOrEmpty(manifestAssetUrl))
            {
                return new ValueTask<string>(result: null);
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

            return new ValueTask<string>(result: assetUrl);

            string withForwardSlash() => manifestAssetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }
    }
}
