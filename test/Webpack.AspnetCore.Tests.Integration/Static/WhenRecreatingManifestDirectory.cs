using System.Threading.Tasks;
using Xunit;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public class WhenRecreatingManifestDirectory
    {
        [Fact]
        public async Task ShouldResolveNewAssetPath()
        {
            // here we're testing that the SUT
            // detects that the manifest folder was recreated,
            // reloads the manifest it and resolves the new asset path

            using (var context = new StaticAssetTestContext())
            {
                var assetPathMapper = context.GetAssetPathMapper();

                // baseline: the asset path mapper should
                // resolve the original asset path
                var assetPath = await assetPathMapper("index.js");
                Assert.Equal(context.AssetPath, assetPath);

                // recreate the manifest directory
                // the asset path mapper should resolve the new asset path
                context.DeployAltAssets();
                await context.WaitForStorageContentsUpdate();
                assetPath = await assetPathMapper("index.js");

                Assert.Equal(context.AltAssetPath, assetPath);
            }
        }
    }
}
