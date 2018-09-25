using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Webpack.AspNetCore.DevServer.Internal;
using Webpack.AspNetCore.Internal;
using Webpack.AspNetCore.Static;
using Webpack.AspNetCore.Static.Internal;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebpackApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebpackDevServer(this IApplicationBuilder app)
            => app.UseWebpack(withDevServer: true);

        public static IApplicationBuilder UseWebpackStatic(this IApplicationBuilder app)
            => app.UseWebpack(withDevServer: false);

        public static IApplicationBuilder UseWebpack(this IApplicationBuilder app, bool withDevServer)
        {
            var context = app.ApplicationServices.GetRequiredService<WebpackContext>();

            if (withDevServer)
            {
                context.AssetSource = WebpackAssetSource.DevServer;
                app.UseMiddleware<DevServerReverseProxyMiddleware>();
            }
            else
            {
                var staticContext = app.ApplicationServices.GetRequiredService<StaticContext>();
                var options = app.ApplicationServices.GetRequiredService<IOptions<StaticOptions>>().Value;
                var service = app.ApplicationServices.GetRequiredService<ManifestStorageService>();

                context.AssetSource = WebpackAssetSource.Static;
                service.Start();

                if (options.UseStaticFileMiddleware)
                {
                    app.UseStaticFiles(staticContext.GetStaticFileOptions());
                }
            }

            return app;
        }
    }
}
