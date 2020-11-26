using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApp
{
    /// <summary>
    /// Startup configuration example with user defined static file options
    /// </summary>
    public class StartupWithStaticFileOptions
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddWebpack().AddStaticOptions(options =>
            {
                options.OnPrepareResponse = responseContext =>
                    responseContext.Context.Response.Headers.Add(
                        key: "Cache-control",
                        value: "public,max-age=31536000"
                    );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
