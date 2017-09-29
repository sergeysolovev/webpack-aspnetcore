using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore.Static.Internal
{
    internal class PhysicalFileManifestReader : IManifestReader
    {
        private readonly StaticContext context;

        public PhysicalFileManifestReader(StaticContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Reads manifest from the physical file, specified in
        /// <see cref="WebpackContext" /> as a deserialized dictionary
        /// </summary>
        /// <returns>
        /// Manifest dictionary if succeeded to read and parse json manifest file,
        /// otherwise false
        /// </returns>
        public async ValueTask<IDictionary<string, string>> ReadAsync()
        {
            var manifestFileInfo = context.GetManifestFileInfo();

            if (!manifestFileInfo.Exists)
            {
                return null;
            }

            try
            {
                // even though we've checked if the manifest
                // file exists by the time we get here the file
                // could become deleted or partially updated
                using (var manifestStream = manifestFileInfo.CreateReadStream())
                using (var manifestReader = new StreamReader(manifestStream))
                {
                    var manifestJson = await manifestReader.ReadToEndAsync();

                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        manifestJson
                    );
                }
            }
            catch (Exception ex) when (shouldHandle(ex))
            {
                return null;
            }

            bool shouldHandle(Exception ex) => ex is IOException || ex is JsonException;
        }
    }
}
