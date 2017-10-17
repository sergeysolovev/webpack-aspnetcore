using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.DevServer
{
    public class WhenRequestingAsset
    {
        [Fact]
        public async Task ShouldServeDevServerAsset()
        {
            // we want to test that webpack-aspnetcore uses
            // specified settings to serve a dev server asset
            // throught the reverse proxy

            using (var context = new DevServerTestContext())
            {
                var assetPathMapper = context.GetAssetPathMapper();
                var assetUrl = await assetPathMapper("index.js");
                var response = await context.Client.GetAsync(assetUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                var etag = response.Headers.ETag;

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("keep-alive", response.Headers.Connection.ToString());
                Assert.NotNull(etag);

                // here we test that the reverse proxy doesn't mess up
                // with etagged request and respond with 304

                var requestWithEtag = new HttpRequestMessage(HttpMethod.Get, assetUrl);
                requestWithEtag.Headers.Add("If-None-Match", etag.ToString());

                response = await context.Client.SendAsync(requestWithEtag);
                responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
                Assert.Equal(0, responseContent.Length);
            }
        }
    }
}
