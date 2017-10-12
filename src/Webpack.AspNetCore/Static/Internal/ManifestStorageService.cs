using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Webpack.AspnetCore.Tests.Integration")]

namespace Webpack.AspNetCore.Static.Internal
{
    internal class ManifestStorageService
    {
        private readonly StaticContext context;
        private readonly PhysicalFileManifestReader reader;
        private readonly ManifestMonitor monitor;
        private readonly ILogger<ManifestStorageService> logger;
        private Task<ManifestStorage> getStorage;

        public ManifestStorageService(
            StaticContext context,
            PhysicalFileManifestReader reader,
            ManifestMonitor monitor,
            ILogger<ManifestStorageService> logger)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));

            this.reader = reader ??
                throw new ArgumentNullException(nameof(reader));

            this.monitor = monitor ??
                throw new ArgumentNullException(nameof(monitor));

            this.logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public Action StorageUpdated { get; set; }

        public Task<ManifestStorage> GetStorageAsync()
        {
            if (getStorage == null)
            {
                throw new InvalidOperationException("The service has not yet started");
            }

            return getStorage;
        }

        public void Start()
        {
            getStorage = setupStorageAsync();

            monitor.ManifestChanged += async () =>
            {
                if (await updateStorageAsync(await GetStorageAsync()))
                {
                    onStorageUpdated();
                }
            };

            monitor.Start();

            async Task<ManifestStorage> setupStorageAsync()
            {
                var storage = new ManifestStorage();
                var manifest = await reader.ReadAsync();

                // if we've failed to retrieve asset manifest
                // in the very beginning, on the application
                // startup, then there is no way to go further
                // so we're throwing the exception
                if (manifest == null)
                {
                    var message = "Failed to retrieve webpack asset manifest. " +
                        $"File path: '{context.ManifestPhysicalPath}'. " +
                        "Check out the file exists and it's a valid json asset manifest";

                    logger.LogError(message);

                    throw new WebpackException(message);
                }

                // set up the manifest storage
                storage.Setup(manifest);
                logger.LogInformation(
                    "Webpack asset manifest storage has been set up. " +
                    $"Keys: ({keysFormatted(manifest.Keys)})"
                );

                return storage;
            }

            async Task<bool> updateStorageAsync(ManifestStorage storage)
            {
                var manifest = await reader.ReadAsync();

                // it's normal if we are failed to read the manifest
                // during the file updates. In this case we're keeping
                // the storage untouched and waiting for the next update
                if (manifest == null)
                {
                    logger.LogDebug($"Webpack asset manifest storage can not be updated now");
                    return false;
                }

                // update the storage
                var updatedKeys = storage.Update(manifest);

                // log if we have any updated or added records
                if (updatedKeys.Any())
                {
                    logger.LogInformation(
                        "Webpack asset manifest storage has been updated. " +
                        $"Updated keys: ({keysFormatted(updatedKeys)})"
                    );

                    return true;
                }

                return false;
            }

            void onStorageUpdated()
            {
                var callback = StorageUpdated;
                if (callback != null)
                {
                    callback.Invoke();
                }
            }

            string keysFormatted(IEnumerable<string> keys) => string.Join(" ", keys);
        }

    }
}
