using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Provides thread-safe storage for the asset manifest
    /// </summary>
    internal class ManifestStorage
    {
        private ConcurrentDictionary<string, string> storage;

        /// <summary>
        /// Gets the asset url by a specified key
        /// </summary>
        /// <param name="assetKey">Asset key</param>
        /// <returns>
        /// Asset url if the key exists in the storage, otherwise null
        /// </returns>
        public string Get(string assetKey)
        {
            storage.TryGetValue(assetKey, out var assetUrl);
            return assetUrl;
        }

        /// <summary>
        /// Sets up the storage with records from a specified
        /// manifest dictionary. The storage can be set up only once.
        /// </summary>
        /// <param name="source">Source manifest dictionary</param>
        public void Setup(IDictionary<string, string> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (storage != null)
            {
                throw new InvalidOperationException("The manifest storage has already been set");
            }

            storage = new ConcurrentDictionary<string, string>(source);
        }

        /// <summary>
        /// Updates the storage with records from a specified
        /// manifest dictionary. The storage has to be set up first.
        /// </summary>
        /// <param name="source">Source manifest dictionary</param>
        /// <returns>Keys of added or updated records</returns>
        public IEnumerable<string> Update(IDictionary<string, string> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (storage == null)
            {
                throw new InvalidOperationException("The manifest storage has not been set yet");
            }

            // A list to keep track of the manifest's keys
            // that were actually updated
            var updated = new List<string>();

            foreach (var record in source)
            {
                storage.AddOrUpdate(
                    key: record.Key,
                    addValueFactory: value =>
                    {
                        updated.Add(record.Key);
                        return record.Value;
                    },
                    updateValueFactory: (key, existingValue) =>
                    {
                        if (!string.Equals(existingValue, record.Value))
                        {
                            updated.Add(record.Key);
                        }
                        return record.Value;
                    }
                );
            }

            return updated;
        }
    }
}
