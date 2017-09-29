using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Webpack.AspNetCore.Internal;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Provides interface for reading the asset manifest
    /// from webpack dev server as a deserialized dictionary.
    /// Supports in-memory caching based on ETag http header,
    /// provided by the dev server.
    /// </summary>
    internal class DevServerManifestReader : IManifestReader
    {
        private class CachedManifest
        {
            public EntityTagHeaderValue Key { get; set; }
            public IDictionary<string, string> Value { get; set; }
        }

        private readonly HttpClient backchannel;
        private CachedManifest cachedManifest;

        public DevServerManifestReader(DevServerContext context, DevServerBackchannelFactory backchannelFactory)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (backchannelFactory == null)
            {
                throw new ArgumentNullException(nameof(backchannelFactory));
            }

            backchannel = backchannelFactory.Create(context.ManifestUri);
            backchannel.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public async ValueTask<IDictionary<string, string>> ReadAsync()
        {
            try
            {
                using (var response = await requestManifest())
                {
                    var etag = response.Headers.ETag;
                    if (TryRestoreCachedManifest(etag, out var manifest))
                    {
                        return manifest;
                    }

                    var manifestJson = await response.Content.ReadAsStringAsync();
                    manifest = JsonConvert.DeserializeObject<Dictionary<string, string>>(manifestJson);

                    CacheManifest(etag, manifest);

                    return manifest;
                }
            }
            catch (Exception ex) when (shouldHandle(ex))
            {
                throw new WebpackDevServerException(
                    "Failed to retrieve the asset manifest from webpack dev server. " +
                    "Check out that the dev server is up and running " +
                    $"and a valid asset manifest is available on '{backchannel.BaseAddress}'",
                    innerException: ex
                );
            }

            Task<HttpResponseMessage> requestManifest() =>
                backchannel.GetAsync(string.Empty, HttpCompletionOption.ResponseHeadersRead);

            bool shouldHandle(Exception ex) =>
                ex is HttpRequestException ||
                ex is JsonException;
        }

        private void CacheManifest(EntityTagHeaderValue key, IDictionary<string, string> value)
        {
            if (key != null && value != null)
            {
                cachedManifest = new CachedManifest { Key = key, Value = value };
            }
        }

        private bool TryRestoreCachedManifest(EntityTagHeaderValue key, out IDictionary<string, string> value)
        {
            if (cachedManifest != null && key != null && key.Equals(cachedManifest.Key))
            {
                value = cachedManifest.Value;
                return true;
            }

            value = null;
            return false;
        }
    }
}
