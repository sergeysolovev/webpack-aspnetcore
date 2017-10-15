using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public class WhenRequestingAsset
    {
        [Fact]
        public async Task ShouldServeItWithValidContentAndHeaders()
        {
            // we want to test that the SUT uses
            // specified settings to serve a static asset
            // with static file middleware

            using (var context = new StaticAssetTestContext())
            {
                var assetPathMapper = context.GetAssetPathMapper();
                var assetPath = await assetPathMapper("index.js");
                var response = await context.Client.GetAsync(assetPath);
                var responseContent = await response.Content.ReadAsStringAsync();
                var fileContent = await context.GetAssetFileContents();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(fileContent, responseContent);
                Assert.Equal("public, max-age=31536000", response.Headers.CacheControl.ToString());
            }
        }
    }
}
