using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;

namespace Webpack.AspNetCore.Internal
{
    internal class WebpackContext
    {
        public WebpackOptions Options { get; private set; }
        public IFileProvider AssetFileProvider { get; private set; }
        public string ManifestFileName { get; private set; }
        public string DevServerHost { get; private set; }
        public Uri DevServerManifestUri { get; private set; }
        public AssetServingMethod Method { get; private set; }
        public PathString StaticRequestPath { get; private set; }

        public IFileInfo GetManifestFileInfo()
        {
            return AssetFileProvider.GetFileInfo(ManifestFileName);
        }

        public StaticFileOptions CreateStaticFileOptions()
        {
            var sourceOptions = Options.StaticOptions;

            if (sourceOptions == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(WebpackOptions.StaticOptions)} can not be null"
                );
            }

            var options = new StaticFileOptions
            {
                FileProvider = AssetFileProvider,
                ContentTypeProvider = sourceOptions.ContentTypeProvider,
                DefaultContentType = sourceOptions.DefaultContentType,
                ServeUnknownFileTypes = sourceOptions.ServeUnknownFileTypes,
                RequestPath = removeTrailingSlash(sourceOptions.RequestPath),
                OnPrepareResponse = sourceOptions.OnPrepareResponse
            };

            return options;

            PathString removeTrailingSlash(string value) => new PathString(value.TrimEnd('/'));
        }

        public WebpackContext(IOptions<WebpackOptions> options, IHostingEnvironment env)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            Options = options.Value;

            var manifestPath = Options.ManifestPath.Value;
            var manifestFileName = manifestPath.Substring(manifestPath.LastIndexOf('/') + 1);
            var manifestRootRelativePath = manifestPath
                .Substring(0, manifestPath.LastIndexOf('/'))
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);
            var manifestRootPath = Path.Combine(env.WebRootPath, manifestRootRelativePath);

            AssetFileProvider = new PhysicalFileProvider(manifestRootPath);
            ManifestFileName = manifestFileName;
            Method = Options.AssetServingMethod;
            StaticRequestPath = Options.StaticOptions.RequestPath;

            if (Method == AssetServingMethod.DevServer)
            {
                DevServerHost = getDevServerHost();
                DevServerManifestUri = getDevServerManifestUri();
            }

            string getDevServerHost() => $"{Options.DevServerOptions.Host}:{Options.DevServerOptions.Port}";

            Uri getDevServerManifestUri()
            {
                var uriBuilder = new UriBuilder
                {
                    Scheme = Options.DevServerOptions.Scheme,
                    Host = Options.DevServerOptions.Host,
                    Port = Options.DevServerOptions.Port,
                    Path = Options.DevServerOptions.PublicPath.Add(Options.ManifestPath)
                };

                return uriBuilder.Uri;
            }
        }
    }
}
