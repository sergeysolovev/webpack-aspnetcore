using Microsoft.Extensions.DependencyInjection;
using Webpack.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebpackApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebpack(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<WebpackContext>();
            var service = app.ApplicationServices.GetService<WebpackManifestStorageService>();

            var options = context.Options;

            if (options.UseStaticFiles)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = context.AssetFileProvider
                });
            }

            service.Start();

            return app;
        }
    }
}
