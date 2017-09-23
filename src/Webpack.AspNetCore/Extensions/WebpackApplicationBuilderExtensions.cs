using Microsoft.Extensions.DependencyInjection;
using Webpack.AspNetCore;
using Webpack.AspNetCore.DevServer;
using Webpack.AspNetCore.Internal;
using Webpack.AspNetCore.Static;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebpackApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebpack(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<WebpackContext>();
            var options = context.Options;

            if (options.UseDevServer)
            {
                app.UseMiddleware<DevServerReverseProxyMiddleware>();
            }
            else
            {
                var service = app.ApplicationServices.GetService<ManifestStorageService>();

                service.Start();

                if (options.UseStaticFiles)
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = context.AssetFileProvider
                    });
                }
            }

            return app;
        }
    }
}
