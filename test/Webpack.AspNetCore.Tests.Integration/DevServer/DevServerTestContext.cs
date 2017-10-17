using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Webpack.AspNetCore.Tests.Integration.DevServer
{
    public class DevServerTestContext : IDisposable
    {
        private readonly TestServer server;
        private readonly HttpClient client;
        private static readonly string publicPath = "/public/";

        public readonly string AssetPath = publicPath + "index.6d4f63cc.js";

        public DevServerTestContext()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(WebSite.GetContentRoot())
                .ConfigureServices(services =>
                {
                    services.AddMvc();

                    services.AddWebpack().AddDevServerOptions(opts =>
                    {
                        opts.PublicPath = publicPath;
                        opts.Port = 8081;
                        opts.ManifestFileName = "webpack-assets.json";
                    });

                    services.AddSingleton<IHttpContextAccessor>(
                        new CustomHttpContextAccessor(publicPath)
                    );
                })
                .Configure(app =>
                {
                    app.UsePathBase(publicPath);
                    app.UseWebpackDevServer();
                    app.UseMvcWithDefaultRoute();
                });

            server = new TestServer(builder);
            client = server.CreateClient();
        }

        public HttpClient Client => client;

        public IServiceProvider Services => server.Host.Services;

        public AssetPathMapper GetAssetPathMapper() =>
            Services.GetRequiredService<AssetPathMapper>();

        public void Dispose()
        {
            server?.Dispose();
            client?.Dispose();
        }
    }
}
