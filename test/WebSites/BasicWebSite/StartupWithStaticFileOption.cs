using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasicWebSite
{
    public class StartupWithStaticFileOption
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack().AddStaticOptions(opts =>
            {
                opts.RequestPath = "/public/";
                opts.ManifestDirectoryPath = "/dist/";
                opts.OnPrepareResponse = respContext =>
                    respContext.Context.Response.Headers.Add(
                        key: "Cache-control",
                        value: "public,max-age=31536000"
                    );
                opts.UseStaticFileMiddleware = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebpackStatic();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
