using System.IO;

namespace Webpack.AspNetCore.Tests.Integration
{
    public static class WebSite
    {
        public static string GetContentRoot()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var webSiteRelativePath = @"../../../../WebSites/BasicWebSite/";

            return Path.Combine(currentDirectory, webSiteRelativePath);
        }
    }
}
