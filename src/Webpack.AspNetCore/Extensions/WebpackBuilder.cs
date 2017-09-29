using System;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class WebpackBuilder : IWebpackBuilder
    {
        public IServiceCollection Services { get; private set; }

        public WebpackBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
