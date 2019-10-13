using Microsoft.Extensions.Configuration;
using NewAlbums.Configuration;
using NewAlbums.Paths;
using NewAlbums.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NewAlbums.Tests.Fixtures
{
    public class SharedFixture
    {
        public IConfiguration Configuration { get; private set; }

        public IPathProvider PathProvider { get; private set; }

        public SharedFixture()
        {
            Configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), "development", addUserSecrets: true);
            PathProvider = new PathProvider(Configuration);

            //Set the DataDirectory which is required by PathProvider
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(currentDirectory, "App_Data"));
        }
    }
}
