using System;
using System.Runtime.Serialization;

namespace Webpack.AspNetCore.DevServer
{
    /// <summary>
    /// Represents error related to webpack dev server
    /// </summary>
    public class WebpackDevServerException : Exception
    {
        public WebpackDevServerException(string message) : base(message)
        {
        }

        public WebpackDevServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebpackDevServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
