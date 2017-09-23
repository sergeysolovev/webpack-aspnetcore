using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Asset url repository
    /// </summary>
    public interface IAssetUrlRepository
    {
        /// <summary>
        /// Gets the asset url by a specified asset key
        /// and context, such as <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        /// <param name="manifestAssetKey">The asset key</param>
        /// <returns>Ready to request asset url</returns>
        ValueTask<string> Get(string manifestAssetKey);
    }
}