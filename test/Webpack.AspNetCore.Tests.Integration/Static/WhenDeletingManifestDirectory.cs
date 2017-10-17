using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public class WhenDeletingManifestDirectory
    {
        [Fact]
        public async Task ShouldResolveAssetPath()
        {
            // after the manifest directory was deleted
            // the asset path should still resolve

            using (var context = new StaticTestContext())
            {
                var assetPathMapper = context.GetAssetPathMapper();

                // baseline: the asset path mapper should
                // resolve the asset path
                var assetPath = await assetPathMapper("index.js");
                Assert.Equal(context.AssetPath, assetPath);

                // recreate the manifest directory
                // the asset path mapper should resolve the new asset path
                context.DeleteAssetDir();
                await context.WaitForStorageUpdate();
                assetPath = await assetPathMapper("index.js");

                Assert.Equal(context.AssetPath, assetPath);
            }
        }
    }
}
