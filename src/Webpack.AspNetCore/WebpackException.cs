using System;
using System.Runtime.Serialization;

namespace Webpack.AspNetCore
{
    /// <summary>
    /// Represents error related to webpack
    /// </summary>
    public class WebpackException : Exception
    {
        public WebpackException(string message) : base(message)
        {
        }

        public WebpackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebpackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
