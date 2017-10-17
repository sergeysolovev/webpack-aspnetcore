using System.IO;

namespace Webpack.AspNetCore.Tests.Integration.Static
{
    public static class CopyUtil
    {
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var source = new DirectoryInfo(sourceDirectory);
            var target = new DirectoryInfo(targetDirectory);

            copyAll(source, target);

            void copyAll(DirectoryInfo sourceDir, DirectoryInfo targetDir)
            {
                Directory.CreateDirectory(targetDir.FullName);

                foreach (FileInfo file in sourceDir.GetFiles())
                {
                    file.CopyTo(Path.Combine(targetDir.FullName, file.Name), true);
                }

                foreach (DirectoryInfo sourceSubdir in sourceDir.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir = targetDir.CreateSubdirectory(sourceSubdir.Name);
                    copyAll(sourceSubdir, nextTargetSubDir);
                }
            }
        }
    }
}
