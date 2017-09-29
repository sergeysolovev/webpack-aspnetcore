using System.Net.Http;

namespace Webpack.AspNetCore.DevServer.Internal
{
    /// <summary>
    /// Provides context for <see cref="DevServerBackchannelFactory"/>.
    /// Acts as a wrapper for <see cref="HttpMessageHandler"/> instance.
    /// <remarks>
    /// This lets avoid conflicting registration of <see cref="HttpMessageHandler"/>
    /// in the DI container, since a service from another library might use it.
    /// </remarks>
    /// </summary>
    internal class DevServerBackchannelFactoryContext
    {
        private readonly HttpClientHandler handlerInstance;

        public DevServerBackchannelFactoryContext()
        {
            handlerInstance = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                UseProxy = false
            };
        }

        public HttpMessageHandler GetMessageHandler() => handlerInstance;
    }
}
