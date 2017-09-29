using System;
using Webpack.AspNetCore.DevServer;
using Webpack.AspNetCore.Static;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebpackBuilderExtensions
    {
        public static IWebpackBuilder AddStaticOptions(this IWebpackBuilder builder, Action<StaticOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder;
        }

        public static IWebpackBuilder AddDevServerOptions(this IWebpackBuilder builder, Action<DevServerOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder;
        }
    }
}
