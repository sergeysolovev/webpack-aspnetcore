using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.Internal
{
    /// <summary>
    /// Provides interface for reading webpack asset manifest
    /// as a deserialized dictionary
    /// </summary>
    internal interface IManifestReader
    {
        ValueTask<IDictionary<string, string>> ReadAsync();
    }
}