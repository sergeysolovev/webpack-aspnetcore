using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Webpack.AspNetCore
{
    internal class ManifestStorage
    {
        private readonly ConcurrentDictionary<string, string> storage;

        public ManifestStorage()
        {
            storage = new ConcurrentDictionary<string, string>();
        }

        public string Get(string assetKey)
        {
            storage.TryGetValue(assetKey, out var assetUrl);
            return assetUrl;
        }

        public IEnumerable<string> Update(IDictionary<string, string> source)
        {
            var changed = new List<string>();

            foreach (var record in source)
            {
                storage.AddOrUpdate(record.Key,
                    (value) =>
                    {
                        changed.Add(record.Key);
                        return record.Value;
                    },
                    (key, existingValue) =>
                    {
                        if (!string.Equals(existingValue, record.Value))
                        {
                            changed.Add(record.Key);
                        }

                        return record.Value;
                    }
                );
            }

            return changed;
        }
    }
}
