using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Webpack.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebpackServiceCollectionExtensions
    {
        public static IServiceCollection AddWebpack(
            this IServiceCollection services,
            Action<WebpackOptions> setupAction)
        {
            services.Configure(setupAction);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<WebpackContext>();
            services.TryAddSingleton<WebpackManifestStorage>();
            services.TryAddSingleton<WebpackManifestReader>();
            services.TryAddSingleton<WebpackManifestStorageService>();
            services.TryAddSingleton<WepbackAssetUrlRepository>();
            services.TryAddSingleton<WebpackAssetMapper>(sp =>
            {
                var repository = sp.GetRequiredService<WepbackAssetUrlRepository>();
                return assetKey => repository.Get(assetKey);
            });

            return services;
        }
    }
}
