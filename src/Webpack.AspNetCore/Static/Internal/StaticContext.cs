using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;

namespace Webpack.AspNetCore.Static.Internal
{
    internal class StaticContext
    {
        private readonly string manifestFileName;
        private readonly string manifestPhysicalPath;
        private readonly IFileProvider fileProvider;
        private readonly StaticOptions options;

        public StaticContext(IOptions<StaticOptions> optionsAccessor, IHostingEnvironment env)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            var options = optionsAccessor.Value;
            var manifestPath = options.ManifestPath.Value;
            var manifestFileName = manifestPath.Substring(manifestPath.LastIndexOf('/') + 1);
            var manifestRootRelativePath = manifestPath
                .Substring(0, manifestPath.LastIndexOf('/'))
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);
            var manifestRootPath = Path.Combine(env.WebRootPath, manifestRootRelativePath);
            var manifestPhysicalPath = Path.Combine(manifestRootPath, manifestFileName);
            var fileProvider = new PhysicalFileProvider(manifestRootPath);

            this.manifestFileName = manifestFileName;
            this.manifestPhysicalPath = manifestPhysicalPath;
            this.fileProvider = fileProvider;
            this.options = options;
        }

        public IFileInfo GetManifestFileInfo() => fileProvider.GetFileInfo(manifestFileName);

        public IChangeToken WatchManifestFile() => fileProvider.Watch(manifestFileName);

        public string ManifestPhysicalPath => manifestPhysicalPath;

        public StaticOptions Options => options;

        public StaticFileOptions GetStaticFileOptions()
        {
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = fileProvider,
                ContentTypeProvider = options.ContentTypeProvider,
                DefaultContentType = options.DefaultContentType,
                ServeUnknownFileTypes = options.ServeUnknownFileTypes,
                RequestPath = removeTrailingSlash(options.RequestPath),
                OnPrepareResponse = options.OnPrepareResponse
            };

            return staticFileOptions;

            PathString removeTrailingSlash(string value) => new PathString(value.TrimEnd('/'));
        }
    }
}
