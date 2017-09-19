using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    public class WebpackManifestReader
    {
        public async Task<IDictionary<string, string>> ReadAsync(WebpackContext context)
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
                        catch (JsonException ex)
                        {
                            // todo: log exception
                            return null;
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
        }
    }
}
