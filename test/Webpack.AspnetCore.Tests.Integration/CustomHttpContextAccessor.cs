using Microsoft.AspNetCore.Http;

namespace Webpack.AspnetCore.Tests.Integration
{
    internal class CustomHttpContextAccessor : IHttpContextAccessor
    {
        public CustomHttpContextAccessor(PathString pathBase)
        {
            HttpContext = new DefaultHttpContext();
            HttpContext.Request.PathBase = pathBase;
        }

        public HttpContext HttpContext { get; set; }
    }
}
