using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Webpack.AspNetCore;
using Webpack.AspNetCore.Static.Internal;

namespace Webpack.AspnetCore.Tests.Integration.Static
{
    internal class StaticAssetTestContext : IDisposable
    {
        public readonly string AssetUrl = "static/js/index.1e09220e.js";
        public readonly string AssetPath = "/public/static/js/index.1e09220e.js";
        public readonly string AltAssetUrl = "static/js/index.7a13033e.js";
        public readonly string AltAssetPath = "/public/static/js/index.7a13033e.js";
        private readonly TaskCompletionSource<bool> waitForStorageUpdate;
        private readonly TaskCompletionSource<bool> waitForStorageContentsUpdate;
        private readonly string webRoot;
        private readonly string assetRoot;
        private const int Timeout = 2000;

        private TestServer server;
        private HttpClient client;

        public StaticAssetTestContext()
        {
            // We're using a separate web root for
            // each context instance to provide isolation
            // for running the tests in parallel
            var webRootDirectory = $"wwwroot-{Guid.NewGuid().ToString()}";

            assetRoot = Path.Combine(Directory.GetCurrentDirectory(), "assets");
            webRoot = Path.Combine(Directory.GetCurrentDirectory(), webRootDirectory);
            waitForStorageUpdate = createTaskCompletionSource();
            waitForStorageContentsUpdate = createTaskCompletionSource();

            startServer();

            void startServer()
            {
                DeployAssets();

                var builder = new WebHostBuilder()
                    .UseWebRoot(webRoot)
                    .ConfigureServices(services =>
                    {
                        services.AddWebpack().AddStaticOptions(opts =>
                        {
                            opts.RequestPath = "/public/";
                            opts.ManifestDirectoryPath = "/dist/";
                            opts.OnPrepareResponse = respContext =>
                                respContext.Context.Response.Headers.Add(
                                    key: "Cache-control",
                                    value: "public,max-age=31536000"
                                );
                            opts.UseStaticFileMiddleware = true;
                        });

                        services.AddSingleton<IHttpContextAccessor>(
                            new CustomHttpContextAccessor(PathString.Empty)
                        );
                    })
                    .Configure(app => app.UseWebpackStatic());

                server = new TestServer(builder);
                setupWaitingForStorageUpdate();

                client = server.CreateClient();
            }

            void setupWaitingForStorageUpdate()
            {
                var storageService = Services.GetRequiredService<ManifestStorageService>();
                storageService.StorageUpdated += onStorageUpdated;
                storageService.StorageContentsUpdated += onStorageContentsUpdated;

                void onStorageUpdated()
                {
                    waitForStorageUpdate.TrySetResult(true);
                    storageService.StorageUpdated -= onStorageUpdated;
                }

                void onStorageContentsUpdated()
                {
                    waitForStorageContentsUpdate.TrySetResult(true);
                    storageService.StorageContentsUpdated -= onStorageContentsUpdated;
                }
            }

            TaskCompletionSource<bool> createTaskCompletionSource() =>
                new TaskCompletionSource<bool>(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
        }

        public IServiceProvider Services => server.Host.Services;

        public HttpClient Client => client;

        public void DeployAssets() => DeployAssetsToOutputFolder("dist");

        public void DeployAltAssets() => DeployAssetsToOutputFolder("dist-alt");

        public void DeleteAssetDir()
        {
            var distDst = Path.Combine(webRoot, "dist");

            if (Directory.Exists(distDst))
            {
                Directory.Delete(distDst, recursive: true);
            }
        }

        public void DeleteManifest()
        {
            var manifestPath = Path.Combine(webRoot, "dist/manifest.json");
            File.Delete(manifestPath);
        }

        public Task WaitForStorageUpdate() =>
            Task.WhenAny(waitForStorageUpdate.Task, Task.Delay(Timeout));

        public Task WaitForStorageContentsUpdate() =>
            Task.WhenAny(waitForStorageContentsUpdate.Task, Task.Delay(Timeout));

        public AssetPathMapper GetAssetPathMapper() =>
            Services.GetRequiredService<AssetPathMapper>();

        public Task<string> GetAssetFileContents()
        {
            var filePath = Path.Combine(webRoot, $"dist", AssetUrl);
            return File.ReadAllTextAsync(filePath);
        }

        public Task RewriteManifestFileAsync()
        {
            var manifestPath = Path.Combine(webRoot, $"dist/manifest.json");
            var manifestText = File.ReadAllText(manifestPath);
            var newText = manifestText.Replace(AssetUrl, AltAssetUrl);

            return File.WriteAllTextAsync(manifestPath, newText);
        }

        public void Dispose()
        {
            if (server != null)
            {
                server.Dispose();
            }
        }

        private void DeployAssetsToOutputFolder(string sourceDir)
        {
            var distSrc = Path.Combine(assetRoot, sourceDir);
            var distDst = Path.Combine(webRoot, "dist");

            DeleteAssetDir();
            CopyUtil.Copy(distSrc, distDst);
        }
    }
}
