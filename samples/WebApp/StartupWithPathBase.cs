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
    /// unix:    export PUBLIC_PATH=/public/ && npm run start
    /// windows: set "PUBLIC_PATH=/public/" && npm run start
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
            if (env.IsDevelopment())
            {
                app.UsePathBase("/public/");
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevServer();
            }
            else
            {
                throw new System.NotSupportedException(
                    $"{env.EnvironmentName} environment is not supported " +
                    $"for {nameof(StartupWithPathBase)}"
                );
            }
            
            app.UseMvcWithDefaultRoute();
        }
    }
}
