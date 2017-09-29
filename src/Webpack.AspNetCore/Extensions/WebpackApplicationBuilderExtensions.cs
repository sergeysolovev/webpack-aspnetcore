using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Webpack.AspNetCore.DevServer;
using Webpack.AspNetCore.Internal;
using Webpack.AspNetCore.Static;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebpackApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebpackDevServer(this IApplicationBuilder app)
            => UseWebpack(app, withDevServer: true);

        public static IApplicationBuilder UseWebpackStatic(this IApplicationBuilder app)
            => UseWebpack(app, withDevServer: false);

        private static IApplicationBuilder UseWebpack(IApplicationBuilder app, bool withDevServer)
        {
            var context = app.ApplicationServices.GetRequiredService<WebpackContext>();

            if (withDevServer)
            {
                context.Mode = AssetServingMethod.DevServer;
                app.UseMiddleware<DevServerReverseProxyMiddleware>();
            }
            else
            {
                var staticContext = app.ApplicationServices.GetRequiredService<WebpackStaticContext>();
                var options = app.ApplicationServices.GetRequiredService<IOptions<StaticOptions>>().Value;
                var service = app.ApplicationServices.GetRequiredService<ManifestStorageService>();

                context.Mode = AssetServingMethod.Static;
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
