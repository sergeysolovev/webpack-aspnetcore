using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Webpack.AspNetCore;
using Xunit;

namespace Webpack.AspnetCore.Tests.Integration
{
    public class StaticAssetTests
    {
        private readonly string webRoot;
        private readonly IWebHostBuilder builder;
        private readonly TestServer server;
        private readonly HttpClient client;

        public StaticAssetTests()
        {
            webRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot");

            builder = new WebHostBuilder()
                .UseWebRoot(webRoot)
                .ConfigureServices(services => {
                    services.AddWebpack().AddStaticOptions(opts =>
                    {
                        opts.RequestPath = "/public/";
                        opts.ManifestDirectoryPath = "/dist/";
                        opts.OnPrepareResponse = respContext =>
                            respContext.Context.Response.Headers.Add(
                                key: "Cache-control",
                                value: "public,max-age=31536000"
                            );
                        opts.UseStaticFileMiddleware = true;
                    });

                    services.AddSingleton<IHttpContextAccessor>(
                        new CustomHttpContextAccessor(PathString.Empty)
                    );
                })
                .Configure(app => app.UseWebpackStatic());

            server = new TestServer(builder);
            client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldServeStaticAsset()
        {
            // we want to test that webpack-aspnetcore uses
            // specified settings to serve a static asset
            // with static file middleware

            var services = server.Host.Services;
            var assetPathMapper = services.GetRequiredService<AssetPathMapper>();
            var assetUrl = await assetPathMapper("index.js");
            var response = await client.GetAsync(assetUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var filePath = Path.Combine(webRoot, $"dist/static/js/index.1e09220e.js");
            var fileContent = await File.ReadAllTextAsync(filePath);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(fileContent, responseContent);
            Assert.Equal("public, max-age=31536000", response.Headers.CacheControl.ToString());
        }
    }
}
