using Microsoft.AspNetCore.Builder;
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
        public static IWebpackBuilder AddWebpack(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<WebpackContext>();
            services.TryAddScoped<AssetPathMapper>(sp =>
            {
                var context = sp.GetRequiredService<WebpackContext>();
                var withDevServer = (context.Mode == AssetServingMethod.DevServer);

                var repository = withDevServer ?
                    sp.GetRequiredService<DevServerAssetPathRepository>() as IAssetPathRepository :
                    sp.GetRequiredService<StaticAssetPathRepository>() as IAssetPathRepository;

                return assetKey => repository.Get(assetKey);
            });

            // dev server services
            services.TryAddTransient<DevServerBackchannelFactory>();
            services.TryAddSingleton<DevServerBackchannelFactoryContext>();
            services.TryAddSingleton<DevServerManifestReader>();
            services.TryAddSingleton<DevServerOptions>();
            services.TryAddSingleton<DevServerContext>();
            services.TryAddScoped<DevServerAssetPathRepository>();
            
            // static services
            services.TryAddSingleton<ManifestStorage>();
            services.TryAddSingleton<PhysicalFileManifestReader>();
            services.TryAddSingleton<ManifestStorageService>();
            services.TryAddSingleton<StaticOptions>();
            services.TryAddSingleton<StaticContext>();
            services.TryAddScoped<StaticAssetPathRepository>();

            return new WebpackBuilder(services);
        }
    }
}
