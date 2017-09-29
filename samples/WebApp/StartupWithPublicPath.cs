using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example, showing how webpack-aspnetcore
    /// works with non-empty public path for static and dev server assets
    /// To use this configuration, webpack's output.publicPath
    /// has to be set.
    /// 
    /// dev server:
    /// unix:    export PUBLIC_PATH=/public/ && npm run start
    /// windows: set "PUBLIC_PATH=/public/" && npm run start
    /// 
    /// static:
    /// unix:    export PUBLIC_PATH=/public/assets/ && npm run build
    /// windows: set "PUBLIC_PATH=/public/assets/" && npm run build
    /// </summary>
    public class StartupWithPublicPath
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack()
                .AddDevServerOptions(options => options.PublicPath = "/public/")
                .AddStaticOptions(options => options.RequestPath = "/assets/");
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

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
