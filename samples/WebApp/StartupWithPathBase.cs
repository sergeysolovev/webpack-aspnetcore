using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example of how to configure webpack
    /// integration with non-empty path base (public path) for dev server
    /// To use this configuration, webpack dev server has to be started
    /// with public path set to /public/:
    /// unix:    export PUBLIC_PATH=/public/ && npm run start|build
    /// windows: set "PUBLIC_PATH=/public/" && npm run start|build
    /// </summary>
    public class StartupWithPathBase
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack(options =>
                options.ConfigureDevServer(opts => opts.PublicPath = "/public/")
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UsePathBase("/public/");

            if (env.IsDevelopment())
            {
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
