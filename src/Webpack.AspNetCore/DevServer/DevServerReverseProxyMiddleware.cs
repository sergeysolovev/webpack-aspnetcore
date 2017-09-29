using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webpack.AspNetCore.DevServer
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
            IOptions<DevServerOptions> optionsAccessor,
            DevServerBackchannelFactory backchannelFactory,
            RequestDelegate next)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (backchannelFactory == null)
            {
                throw new ArgumentNullException(nameof(backchannelFactory));
            }

            var options = optionsAccessor.Value;

            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.devServerHost = $"{options.Host}:{options.Port}";
            this.backchannel = createBackchannel();

            HttpClient createBackchannel()
            {
                var baseAddress = new UriBuilder
                {
                    Host = options.Host,
                    Port = options.Port,
                    Scheme = options.Scheme
                };

                return backchannelFactory.Create(baseAddress.Uri);
            }
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

