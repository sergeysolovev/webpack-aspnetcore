using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public class WhenChangingManifestContents
    {
        [Fact]
        public async Task ShouldResolveNewAssetPath()
        {
            // here we're testing that the SUT
            // detects the changes to the manifest file,
            // reloads it and resolves the new asset path

            using (var context = new StaticTestContext())
            {
                var assetPathMapper = context.GetAssetPathMapper();

                // baseline: the asset path mapper should
                // resolve the original asset path
                var assetPath = await assetPathMapper("index.js");
                Assert.Equal(context.AssetPath, assetPath);

                // rewrite the manifest file
                // the asset path mapper should resolve the new asset path
                await context.RewriteManifestFileAsync();
                await context.WaitForStorageContentsUpdate();
                assetPath = await assetPathMapper("index.js");

                Assert.Equal(context.AltAssetPath, assetPath);
            }
        }
    }
}
