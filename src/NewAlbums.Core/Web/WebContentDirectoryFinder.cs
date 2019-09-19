using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NewAlbums.Web
{
    //Credit: https://github.com/aspnetboilerplate
    public static class WebContentDirectoryFinder
    {
        public static string CalculateContentRootFolder()
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(WebContentDirectoryFinder).GetTypeInfo().Assembly.Location);
            if (coreAssemblyDirectoryPath == null)
            {
                throw new Exception("Could not find location of core assembly!");
            }

            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!DirectoryContains(directoryInfo.FullName, "NewAlbums.sln"))
            {
                if (directoryInfo.Parent == null)
                {
                    throw new Exception("Could not find content root folder!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            var webFolder = Path.Combine(directoryInfo.FullName, "src", "NewAlbums.Web");
            if (Directory.Exists(webFolder))
            {
                return webFolder;
            }

            throw new Exception("Could not find root folder of the web project!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
