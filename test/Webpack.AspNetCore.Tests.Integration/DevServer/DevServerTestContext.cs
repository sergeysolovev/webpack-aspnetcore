using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using BasicWebSite;

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
                .ConfigureServices(services =>
                    services.AddSingleton<IHttpContextAccessor>(
                        new CustomHttpContextAccessor(publicPath)
                    )
                )
                .UseStartup<Startup>()
                .UseContentRoot(WebSite.GetContentRoot());

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
