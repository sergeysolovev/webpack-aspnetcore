using System;
using System.Net.Http;

namespace Webpack.AspNetCore.DevServer.Internal
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

        public HttpClient Create(Uri baseAddress, bool validateCert)
        {
            var handler = context.GetMessageHandler();

            // We don't always want to validate when connecting locally. This is only for dev anyways, right?
            if (!validateCert)
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true;

            return new HttpClient(handler, disposeHandler: false)
            {
                BaseAddress = baseAddress
            };
        }
    }
}
