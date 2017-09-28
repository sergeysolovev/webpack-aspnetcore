using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example with user defined static file options
    /// To use this configuration, webpack static assets have to be built
    /// with public path set to /public/:
    /// unix:    export PUBLIC_PATH=/public/ && npm run build|start
    /// windows: set "PUBLIC_PATH=/public/" && npm run build|start
    /// </summary>
    public class StartupWithStaticFileOptions
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack(options =>
            {
                options.ConfigureStatic(opts => {
                    opts.RequestPath = "/public/";
                    opts.OnPrepareResponse = responseContext =>
                        responseContext.Context.Response.Headers.Add(
                            key: "Cache-control",
                            value: "public,max-age=31536000"
                        );
                });

                options.ConfigureDevServer(opts =>
                    opts.PublicPath = "/public/"
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UsePathBase("/public/");
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevServer();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseWebpackStatic();
            }

            app.UseMvcWithDefaultRoute();
        }
    }
}
