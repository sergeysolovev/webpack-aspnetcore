using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public class WhenRequestingRazorViewWithInvalidAssetKey
    {
        [Fact]
        public async Task ShouldSkipSrcAttribute()
        {
            // here we're testing that the asset path mapper
            // correctly injected a valid asset path
            // into the razor view

            using (var context = new StaticTestContext())
            {
                var response = await context.Client.GetAsync("/Test/IndexInvalidAssetKey");
                var responseContent = await response.Content.ReadAsStringAsync();
                var content = $"<script></script>";

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(content, responseContent);
            }
        }
    }
}
