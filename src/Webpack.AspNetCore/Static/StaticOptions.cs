using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;

namespace Webpack.AspNetCore.Static
{
    /// <summary>
    /// Represents a limited set of options from <see cref="StaticFileOptions" />
    /// <see cref="SharedOptionsBase.FileProvider" /> is excluded
    /// </summary>
    public class StaticOptions
    {
        public StaticOptions()
        {
            ManifestPath = new PathString("/dist/manifest.json");
            UseStaticFileMiddleware = true;
            OnPrepareResponse = _ => { };
        }

        /// <summary>
        /// The asset manifest path within the application's web root path
        /// Default: /dist/manifest.json
        /// </summary>
        public PathString ManifestPath { get; set; }

        /// <summary>
        /// Determines whether to use <see cref="Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware"/>
        /// to serve static assets from the root of <see cref="ManifestPath"/>.
        /// Default: <c>true</c>
        /// </summary>
        public bool UseStaticFileMiddleware { get; set; }

        /// <summary>
        /// The relative request path that maps to static resources.
        /// </summary>
        public PathString RequestPath { get; set; }

        /// <summary>
        /// Used to map files to content-types.
        /// </summary>
        public IContentTypeProvider ContentTypeProvider { get; set; }

        /// <summary>
        /// The default content type for a request if the ContentTypeProvider cannot determine one.
        /// None is provided by default, so the client must determine the format themselves.
        /// http://www.w3.org/Protocols/rfc2616/rfc2616-sec7.html#sec7
        /// </summary>
        public string DefaultContentType { get; set; }

        /// <summary>
        /// If the file is not a recognized content-type should it be served?
        /// Default: false.
        /// </summary>
        public bool ServeUnknownFileTypes { get; set; }

        /// <summary>
        /// Called after the status code and headers have been set, but before the body has been written.
        /// This can be used to add or change the response headers.
        /// </summary>
        public Action<StaticFileResponseContext> OnPrepareResponse { get; set; }
    }
}
