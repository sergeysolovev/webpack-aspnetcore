using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.Static
{
    /// <summary>
    /// Implementation of <see cref="IAssetPathRepository"/>
    /// for static webpack assets. Uses <see cref="HttpContext"/>
    /// to determine asset urls' path base.
    /// </summary>
    internal class StaticAssetPathRepository : IAssetPathRepository
    {
        private readonly StaticContext context;
        private readonly ManifestStorage storage;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<StaticAssetPathRepository> logger;

        public StaticAssetPathRepository(
            StaticContext context,
            ManifestStorage storage,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StaticAssetPathRepository> logger)
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

        public ValueTask<string> Get(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                return new ValueTask<string>(result: null);
            }

            var assetUrl = storage.Get(assetKey) ?? storage.Get(withForwardSlash());

            if (string.IsNullOrEmpty(assetUrl))
            {
                return new ValueTask<string>(result: null);
            }

            var pathBase = makePath(httpContextAccessor.HttpContext.Request.PathBase);
            var assetRelativePath = makePath(assetUrl);
            var publicPath = pathBase.Add(context.Options.RequestPath);
            var assetPath = publicPath.Add(assetRelativePath).Value;

            logger.LogDebug(
                $"Mapped webpack asset key '{assetKey}' to the path '{assetPath}'. " +
                $"Public path: '{publicPath}'."
            );

            return new ValueTask<string>(result: assetPath);

            string withForwardSlash() => assetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }
    }
}
