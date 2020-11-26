using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BasicWebSite
{
    public class Startup
    {
        private static readonly string publicPath = "/public/";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddWebpack().AddDevServerOptions(opts =>
            {
                opts.PublicPath = publicPath;
                opts.Port = 8081;
                opts.ManifestFileName = "webpack-assets.json";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(publicPath);
            app.UseWebpackDevServer();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
