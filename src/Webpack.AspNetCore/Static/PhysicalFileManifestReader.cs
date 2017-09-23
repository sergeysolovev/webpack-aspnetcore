using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore.Static
{
    internal class PhysicalFileManifestReader : IManifestReader
    {
        private readonly WebpackContext context;

        public PhysicalFileManifestReader(WebpackContext context)
        {
            this.context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async ValueTask<IDictionary<string, string>> ReadAsync()
        {
            var manifestFileInfo = context.GetManifestFileInfo();
            if (!manifestFileInfo.Exists)
            {
                return null;
            }

            try
            {
                using (var manifestStream = manifestFileInfo.CreateReadStream())
                {
                    using (var manifestReader = new StreamReader(manifestStream))
                    {
                        var manifestJson = await manifestReader.ReadToEndAsync();

                        try
                        {
                            return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                manifestJson
                            );
                        }
                        catch (JsonException)
                        {
                            return null;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
