using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Asset path repository
    /// </summary>
    public interface IAssetPathRepository
    {
        /// <summary>
        /// Gets the asset path by a specified asset key
        /// and context, such as <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        /// <param name="manifestAssetKey">The asset key</param>
        /// <returns>Ready to be served asset path if the key exists, othewise null</returns>
        ValueTask<string> Get(string manifestAssetKey);
    }
}
