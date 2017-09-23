using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Webpack.AspNetCore;
using Webpack.AspNetCore.DevServer;
using Webpack.AspNetCore.Internal;
using Webpack.AspNetCore.Static;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebpackServiceCollectionExtensions
    {
        public static IServiceCollection AddWebpack(this IServiceCollection services, Action<WebpackOptions> setupAction)
        {
            createAndConfigureOptions(out WebpackOptions options);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<WebpackContext>();

            if (options.UseDevServer)
            {
                services.TryAddTransient<DevServerBackchannelFactory>();
                services.TryAddSingleton<DevServerBackchannelFactoryContext>();
                services.TryAddSingleton<DevServerManifestReader>();
                services.TryAddScoped<DevServerManifestRepository>();
                services.TryAddScoped<AssetUrlMapper>(sp =>
                {
                    var repository = sp.GetRequiredService<DevServerManifestRepository>();
                    return assetKey => repository.Get(assetKey);
                });
            }
            else
            {
                services.TryAddSingleton<ManifestStorage>();
                services.TryAddSingleton<PhysicalFileManifestReader>();
                services.TryAddSingleton<ManifestStorageService>();
                services.TryAddScoped<StaticAssetUrlRepository>();
                services.TryAddScoped<AssetUrlMapper>(sp =>
                {
                    var repository = sp.GetRequiredService<StaticAssetUrlRepository>();
                    return assetKey => repository.Get(assetKey);
                });
            }

            return services;

            void createAndConfigureOptions(out WebpackOptions webpackOptions)
            {
                webpackOptions = new WebpackOptions();

                setupAction?.Invoke(options);
                services.AddSingleton(Options.Options.Create(options));
            }
        }
    }
}
