using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example with user defined static file options
    /// To use this configuration, webpack static assets have to be built
    /// with public path set to /public/:
    /// unix:    export PUBLIC_PATH=/public/ && npm run build
    /// windows: set "PUBLIC_PATH=/public/" && npm run build
    /// </summary>
    public class StartupWithStaticFileOptions
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack(options =>
            {
                options.UseStaticFiles(opts => {
                    opts.RequestPath = "/public";
                    opts.OnPrepareResponse = responseContext =>
                        responseContext.Context.Response.Headers.Add(
                            key: "Cache-control",
                            value: "public,max-age=31536000"
                        );
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseWebpack();
            app.UseMvcWithDefaultRoute();
        }
    }
}
