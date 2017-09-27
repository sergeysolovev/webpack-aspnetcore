using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Webpack.AspNetCore;
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
            if (withDevServer)
            {
                app.ApplicationServices
                    .GetRequiredService<IOptions<WebpackOptions>>()
                    .Value
                    .AssetServingMethod = AssetServingMethod.DevServer;

                app.UseMiddleware<DevServerReverseProxyMiddleware>();
            }
            else
            {
                app.ApplicationServices
                    .GetRequiredService<ManifestStorageService>()
                    .Start();
            }

            var context = app
                .ApplicationServices
                .GetRequiredService<WebpackContext>();

            if (context.Options.UseStaticFileMiddleware)
            {
                app.UseStaticFiles(context.CreateStaticFileOptions());
            }

            return app;
        }
    }
}
