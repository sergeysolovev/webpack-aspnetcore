using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.DevServer.Internal
{
    /// <summary>
    /// A reverse proxy implementation for webpack dev server.
    /// Proxies webpack assets' requests and bypasses all the rest.
    /// </summary>
    internal class DevServerReverseProxyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly HttpClient backchannel;
        private readonly string devServerHost;

        public DevServerReverseProxyMiddleware(
            DevServerContext context,
            DevServerBackchannelFactory backchannelFactory,
            RequestDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (backchannelFactory == null)
            {
                throw new ArgumentNullException(nameof(backchannelFactory));
            }

            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.devServerHost = context.DevServerHost;
            this.backchannel = backchannelFactory.Create(context.DevServerUri, context.Options.ValidateServerCert);
        }

        public async Task Invoke(HttpContext context, DevServerAssetPathRepository repository)
        {
            var userRequest = context.Request;

            if (await repository.HasMatchingPath(userRequest.Path))
            {
                var response = context.Response;

                using (var upstreamResponse = await requestUpstreamAsync())
                {
                    response.StatusCode = (int)upstreamResponse.StatusCode;

                    foreach (var header in upstreamResponse.Headers)
                    {
                        response.Headers[header.Key] = header.Value.ToArray();
                    }

                    foreach (var header in upstreamResponse.Content.Headers)
                    {
                        response.Headers[header.Key] = header.Value.ToArray();
                    }

                    response.Headers.Remove("transfer-encoding");

                    await upstreamResponse.Content.CopyToAsync(response.Body);
                }
            }
            else
            {
                await next(context);
            }

            Task<HttpResponseMessage> requestUpstreamAsync() =>
                backchannel.SendAsync(
                    createProxyRequest(),
                    HttpCompletionOption.ResponseHeadersRead,
                    context.RequestAborted
                );

            HttpRequestMessage createProxyRequest()
            {
                var proxyRequest = new HttpRequestMessage();
                var requestUri = userRequest.PathBase
                    .Add(userRequest.Path)
                    .Add(userRequest.QueryString);

                proxyRequest.RequestUri = new Uri(requestUri, UriKind.Relative);
                proxyRequest.Method = new HttpMethod(userRequest.Method);

                foreach (var header in userRequest.Headers)
                {
                    proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }

                proxyRequest.Headers.Host = devServerHost;

                return proxyRequest;
            }
        }
    }
}

