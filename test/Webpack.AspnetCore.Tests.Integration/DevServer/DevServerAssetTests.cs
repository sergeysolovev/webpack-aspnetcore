using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Webpack.AspNetCore;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.DevServer
{
    public class DevServerAssetTests
    {
        private readonly IWebHostBuilder builder;
        private readonly TestServer server;
        private readonly HttpClient client;

        public DevServerAssetTests()
        {
            builder = new WebHostBuilder()
                .ConfigureServices(services => {
                    services.AddWebpack().AddDevServerOptions(opts =>
                    {
                        opts.PublicPath = "/public/";
                        opts.Port = 8081;
                        opts.ManifestFileName = "webpack-assets.json";
                    });

                    services.AddSingleton<IHttpContextAccessor>(
                        new CustomHttpContextAccessor("/public")
                    );
                })
                .Configure(app => app.UseWebpackDevServer());

            server = new TestServer(builder);
            client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldServeDevServerAsset()
        {
            // we want to test that webpack-aspnetcore uses
            // specified settings to serve a dev server asset
            // throught the reverse proxy

            var services = server.Host.Services;
            var assetPathMapper = services.GetRequiredService<AssetPathMapper>();
            var assetUrl = await assetPathMapper("index.js");
            var response = await client.GetAsync(assetUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var etag = response.Headers.ETag;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("keep-alive", response.Headers.Connection.ToString());
            Assert.NotNull(etag);

            // here we test that the reverse proxy doesn't mess up
            // with etagged request and respond with 304

            var requestWithEtag = new HttpRequestMessage(HttpMethod.Get, assetUrl);
            requestWithEtag.Headers.Add("If-None-Match", etag.ToString());

            response = await client.SendAsync(requestWithEtag);
            responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
            Assert.Equal(0, responseContent.Length);
        }
    }
}
