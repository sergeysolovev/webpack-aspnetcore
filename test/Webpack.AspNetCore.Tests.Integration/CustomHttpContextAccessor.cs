using Microsoft.AspNetCore.Http;
using System;

namespace Webpack.AspNetCore.Tests.Integration
{
    internal class CustomHttpContextAccessor : IHttpContextAccessor
    {
        private readonly PathString pathBase;
        private HttpContext context;

        public CustomHttpContextAccessor() : this(PathString.Empty)
        {

        }

        public CustomHttpContextAccessor(PathString pathBase)
        {
            this.pathBase = pathBase.Value.TrimEnd('/');
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
