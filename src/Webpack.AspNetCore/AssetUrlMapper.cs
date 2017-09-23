using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Represents shortcut for <see cref="IAssetUrlRepository.Get(string)"/>
    /// Provides concise syntax for getting asset urls when injected into views
    /// </summary>
    /// <param name="assetKey"></param>
    /// <returns></returns>
    public delegate ValueTask<string> AssetUrlMapper(string assetKey);
}