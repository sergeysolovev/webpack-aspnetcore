using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.Static.Internal
{
    /// <summary>
    /// Implementation of <see cref="IAssetPathRepository"/>
    /// for static webpack assets. Uses <see cref="HttpContext"/>
    /// to determine asset urls' path base.
    /// </summary>
    internal class StaticAssetPathRepository : IAssetPathRepository
    {
        private readonly StaticContext context;
        private readonly ManifestStorageService storageService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<StaticAssetPathRepository> logger;

        public StaticAssetPathRepository(
            StaticContext context,
            ManifestStorageService storageService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StaticAssetPathRepository> logger)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));

            this.storageService = storageService ??
                throw new ArgumentNullException(nameof(storageService));

            this.httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            this.logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<string> Get(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                return null;
            }

            var storage = await storageService.GetStorageAsync();
            var assetUrl = storage.Get(assetKey) ?? storage.Get(withForwardSlash());

            if (string.IsNullOrEmpty(assetUrl))
            {
                return null;
            }

            var pathBase = makePath(httpContextAccessor.HttpContext.Request.PathBase);
            var assetRelativePath = makePath(assetUrl);
            var publicPath = pathBase.Add(context.Options.RequestPath);
            var assetPath = publicPath.Add(assetRelativePath).Value;

            logger.LogDebug(
                $"Mapped webpack asset key '{assetKey}' to the path '{assetPath}'. " +
                $"Public path: '{publicPath}'."
            );

            return assetPath;

            string withForwardSlash() => assetKey.Replace('/', '\\');
            PathString makePath(string value) => new PathString('/' + value.Trim('/'));
        }
    }
}
