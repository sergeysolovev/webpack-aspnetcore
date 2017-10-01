using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.Static.Internal
{
    /// <summary>
    /// Provides an interface for monitoring the manifest file
    /// and directory changes
    /// </summary>
    internal class ManifestMonitor
    {
        private readonly StaticContext context;
        private readonly ILogger<ManifestMonitor> logger;

        public ManifestMonitor(StaticContext context, ILogger<ManifestMonitor> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts monitoring changes to the manifest or it's directory
        /// and awaits until any changes are made
        /// </summary>
        /// <returns>
        /// <c>true</c> if the manifest file or directory have been recreated
        /// or the manifest contents have been changed, otherwise <c>false</c>
        /// </returns>
        public Task<bool> WaitForChangesAsync()
        {
            var manifestPath = context.ManifestPhysicalPath;
            var manifestDir = Path.GetDirectoryName(manifestPath);
            var manifestDirName = Path.GetFileName(manifestDir);
            var manifestDirParent = Directory.GetParent(manifestDir).FullName;
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var watcher = new FileSystemWatcher();

            setup();
            start();

            return taskCompletionSource.Task;

            void onChange(object sender, FileSystemEventArgs e)
            {
                if (e.FullPath == manifestDir)
                {
                    logger.LogDebug(
                        "The manifest directory has been modified. " +
                        $"Directory: '{e.FullPath}'. Change type: {e.ChangeType}"
                    );

                    taskCompletionSource.TrySetResult(result: (e.ChangeType != WatcherChangeTypes.Deleted));
                    tearDown();
                }

                if (e.FullPath == manifestPath)
                {
                    logger.LogDebug(
                        "The manifest file has been modified. " +
                        $"File: '{e.FullPath}'. Change type: {e.ChangeType}"
                    );

                    taskCompletionSource.TrySetResult(result: (e.ChangeType != WatcherChangeTypes.Deleted));
                    tearDown();
                }
            }

            void onError(object sender, ErrorEventArgs e)
            {
                logger.LogError(e.GetException(),
                    "An error occurred while watching the manifest directory changes. " +
                    $"Directory: {manifestDir}."
                );

                taskCompletionSource.TrySetResult(true);
                tearDown();
            }

            void setup()
            {
                watcher.Path = manifestDirParent;
                watcher.IncludeSubdirectories = true;
                watcher.Created += onChange;
                watcher.Renamed += onChange;
                watcher.Deleted += onChange;
                watcher.Error += onError;
            }

            void tearDown()
            {
                watcher.Created -= onChange;
                watcher.Renamed -= onChange;
                watcher.Deleted -= onChange;
                watcher.Error -= onError;
                watcher.Dispose();
            }

            void start() => watcher.EnableRaisingEvents = true;
        }
    }
}
