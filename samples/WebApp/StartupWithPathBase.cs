using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example of how to configure webpack
    /// integration with non-empty path base (public path) for dev server
    /// To use this configuration, webpack static assets have to be built
    /// with public path set to /public/:
    /// unix:    export PUBLIC_PATH=/public/ && npm run build
    /// windows: set "PUBLIC_PATH=/public/" && npm run build
    /// </summary>
    public class StartupWithPathBase
    {
        public const string PublicPath = "/public/";

        public StartupWithPathBase(IHostingEnvironment env)
            => Environment = env;

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack(options =>
                options.UseDevServer(opts => opts.PublicPath = PublicPath)
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UsePathBase(PublicPath);
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
