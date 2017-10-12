using Microsoft.AspNetCore.Http;
using System;

namespace Webpack.AspnetCore.Tests.Integration
{
    internal class CustomHttpContextAccessor : IHttpContextAccessor
    {
        private readonly PathString pathBase;
        private HttpContext context;

        public CustomHttpContextAccessor(PathString pathBase)
        {
            this.pathBase = pathBase;
        }

        public HttpContext HttpContext
        {
            get
            {
                if (context == null)
                {
                    context = new DefaultHttpContext();
                    context.Request.PathBase = pathBase;
                }
                
                return context;
            }
            set
            {
                context = value;
            }
        }
    }
}
