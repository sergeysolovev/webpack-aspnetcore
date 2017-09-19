using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Webpack.AspNetCore
{
    public class WebpackManifestStorageService
    {
        private readonly WebpackContext context;
        private readonly WebpackManifestReader reader;
        private readonly WebpackManifestStorage storage;
        private readonly ILogger<WebpackManifestStorageService> logger;

        public WebpackManifestStorageService(
            WebpackContext context,
            WebpackManifestReader reader,
            WebpackManifestStorage storage,
            ILogger<WebpackManifestStorageService> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Start()
        {
            updateStorage().Wait();

            Task.Run(async () =>
            {
                while (true)
                {
                    await waitForChanges();
                    await updateStorage();
                }
            });

            async Task updateStorage()
            {
                var manifest = await reader.ReadAsync(context);
                if (manifest != null)
                {
                    var changedKeys = storage.Update(manifest);
                    if (changedKeys.Any())
                    {
                        var changed = String.Join(" ", changedKeys);

                        logger.LogInformation(
                            $"Updated webpack asset manifest storage. Keys: {changed}"
                        );

                        return;
                    }
                }

                logger.LogInformation($"webpack asset manifest storage was not updated");
            }

            async Task waitForChanges()
            {
                var token = context.AssetFileProvider.Watch(context.ManifestFileName);
                var taskCompletionSource = new TaskCompletionSource<object>();

                token.RegisterChangeCallback(
                    state => ((TaskCompletionSource<object>)state).TrySetResult(null),
                    taskCompletionSource
                );

                await taskCompletionSource.Task.ConfigureAwait(false);

                logger.LogInformation(
                    $"The contents of webpack asset manifest have been changed. Filename: '{context.ManifestFileName}'."
                );
            }
        }
    }
}
