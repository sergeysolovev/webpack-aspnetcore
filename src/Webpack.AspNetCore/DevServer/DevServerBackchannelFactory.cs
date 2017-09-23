using System;
using System.Net.Http;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Creates instances of <see cref="HttpClient"/>
    /// for backend requests to the dev server
    /// </summary>
    internal class DevServerBackchannelFactory
    {
        private readonly DevServerBackchannelFactoryContext context;

        public DevServerBackchannelFactory(DevServerBackchannelFactoryContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public HttpClient Create(Uri baseAddress)
        {
            var handler = context.GetMessageHandler();
            return new HttpClient(handler, disposeHandler: false)
            {
                BaseAddress = baseAddress
            };
        }
    }
}
