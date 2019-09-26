using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NewAlbums.Utils
{
    public class FileUtils
    {
        public static string GetFileSafeName(string str)
        {
            //Replace non-unicode items
            str = Regex.Replace(str, @"[^\u0020-\u007E]", "");

            //First replace slash characters with hyphens to try to keep readability
            str = str.Replace("/", "-");
            str = str.Replace("\\", "-");

            //Replace space characters with hyphens
            str = str.Replace(" ", "-");

            //Replace dangerous web request characters
            str = str.Replace("&", "");
            str = str.Replace("?", "");
            str = str.Replace("#", "");
            str = str.Replace(":", "");

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                str = str.Replace(c.ToString(), "");
            }

            //Increase length if necessary
            if (String.IsNullOrWhiteSpace(str))
            {
                str = Guid.NewGuid().ToString().Substring(0, 5);
            }

            return str;
        }

        public static string GetExtensionWithDot(string filename)
        {
            int lastDotIndex = filename.LastIndexOf(".");
            if (lastDotIndex >= 0)
                return filename.Substring(lastDotIndex);

            return null;
        }

        public static string GetFilenameFromUrl(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
                return null;

            //Handle relative urls
            if (url.StartsWith("/"))
            {
                url = "http://localhost" + url;
            }

            Uri uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }
    }
}
