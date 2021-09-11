using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NewAlbums.Configuration;
using NewAlbums.Utils;

namespace NewAlbums.Paths
{
    public class PathProvider : IPathProvider
    {
        private readonly IConfiguration _configuration;

        private const string EmailsFolderName = "Emails";

        public PathProvider(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Files on disk

        public string GetAbsoluteEmailsFolderPath()
        {
            //Safe way to get the App_Data directory without needing to reference System.Web
            object dataDirectoryObj = AppDomain.CurrentDomain.GetData("DataDirectory");
            if (dataDirectoryObj != null)
            {
                return Path.Combine(dataDirectoryObj.ToString(), EmailsFolderName);
            }

            return null;
        }

        #endregion

        #region Public URLs

        /// <summary>
        /// relativeWebFilePath should be relative to the WebRootPath, ie the wwwroot folder
        /// </summary>
        public string GetAbsoluteUrl(string relativeWebFilePath)
        {
            string websiteRoot = _configuration[AppSettingKeys.App.FrontEndRootUrl];

            return websiteRoot.EnsureEndsWith('/') + relativeWebFilePath.RemovePreFix("/", "~/");
        }

        #endregion
    }
}
