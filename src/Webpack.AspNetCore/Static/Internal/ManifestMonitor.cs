using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.Static.Internal
{
    /// <summary>
    /// Provides an interface for monitoring changes of
    /// the manifest file and it's directory
    /// </summary>
    internal class ManifestMonitor : IDisposable
    {
        private readonly StaticContext context;
        private readonly ILogger<ManifestMonitor> logger;

        private string manifestDir;
        private string manifestPath;
        private string manifestDirName;
        private string manifestDirParent;
        private FileSystemWatcher watcher;

        public ManifestMonitor(StaticContext context, ILogger<ManifestMonitor> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Calls back when manifest or it's directory changes
        /// </summary>
        public Action ManifestChanged { get; set; }

        /// <summary>
        /// Starts monitoring changes to the manifest or it's directory
        /// </summary>
        /// <returns>
        /// </returns>
        public void Start()
        {
            if (watcher != null)
            {
                throw new InvalidOperationException(
                    "The manifest monitor has already been started"
                );
            }

            watcher = new FileSystemWatcher();

            setup();
            start();

            void setup()
            {
                manifestPath = context.ManifestPhysicalPath;
                manifestDir = Path.GetDirectoryName(manifestPath);
                manifestDirName = Path.GetFileName(manifestDir);
                manifestDirParent = Directory.GetParent(manifestDir).FullName;
                watcher.Path = manifestDirParent;
                watcher.IncludeSubdirectories = true;
                watcher.Created += OnChange;
                watcher.Changed += OnChange;
                watcher.Renamed += OnChange;
                watcher.Deleted += OnChange;
                watcher.Error += OnError;
            }

            void start() => watcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.Created -= OnChange;
                watcher.Changed -= OnChange;
                watcher.Renamed -= OnChange;
                watcher.Deleted -= OnChange;
                watcher.Error -= OnError;
                watcher.Dispose();
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == manifestDir)
            {
                logger.LogDebug(
                    "The manifest directory has been modified. " +
                    $"Directory: '{e.FullPath}'. Change type: {e.ChangeType}"
                );

                if (e.ChangeType != WatcherChangeTypes.Deleted)
                {
                    onManifestChanged();
                }
            }

            if (e.FullPath == manifestPath)
            {
                logger.LogDebug(
                    "The manifest file has been modified. " +
                    $"File: '{e.FullPath}'. Change type: {e.ChangeType}"
                );

                if (e.ChangeType != WatcherChangeTypes.Deleted)
                {
                    onManifestChanged();
                }
            }

            void onManifestChanged()
            {
                var callback = ManifestChanged;
                if (callback != null)
                {
                    callback.Invoke();
                }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            logger.LogError(e.GetException(),
                "An error occurred while watching the manifest directory changes. " +
                $"Directory: {manifestDir}."
            );
        }
        
        public Task<bool> WaitForChangesAsync()
        {
            var manifestPath = context.ManifestPhysicalPath;
            var manifestDir = Path.GetDirectoryName(manifestPath);
            var manifestDirName = Path.GetFileName(manifestDir);
            var manifestDirParent = Directory.GetParent(manifestDir).FullName;
            var watcher = new FileSystemWatcher();
            var taskCompletionSource = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

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
                watcher.Changed += onChange;
                watcher.Renamed += onChange;
                watcher.Deleted += onChange;
                watcher.Error += onError;
            }

            void tearDown()
            {
                watcher.Created -= onChange;
                watcher.Changed -= onChange;
                watcher.Renamed -= onChange;
                watcher.Deleted -= onChange;
                watcher.Error -= onError;
                watcher.Dispose();
            }

            void start() => watcher.EnableRaisingEvents = true;
        }
    }
}
