using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Represents shortcut for <see cref="IAssetPathRepository.Get(string)"/>
    /// Provides concise syntax for getting asset urls when injected into views
    /// </summary>
    /// <param name="assetKey"></param>
    /// <returns>Asset path</returns>
    public delegate ValueTask<string> AssetPathMapper(string assetKey);
}
