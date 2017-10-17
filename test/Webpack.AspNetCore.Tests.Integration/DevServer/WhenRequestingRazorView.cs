using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.DevServer
{
    public class WhenRequestingRazorView
    {
        [Fact]
        public async Task ShouldInjectValidAssetPath()
        {
            // here we're testing that the asset path mapper
            // correctly injected a valid asset path
            // into the razor view

            using (var context = new DevServerTestContext())
            {
                var response = await context.Client.GetAsync("/public/Test/Index");
                var responseContent = await response.Content.ReadAsStringAsync();
                var content = $"<script src=\"{context.AssetPath}\"></script>";

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(content, responseContent);
            }
        }
    }
}
