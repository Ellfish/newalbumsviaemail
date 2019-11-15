using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NewAlbums.Logging
{
    //Adapted from: https://stackify.com/net-core-loggerfactory-use-correctly/
    public class NewAlbumsLogging
    {
        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(ILoggerFactory factory, string configFileName = "log4net.config")
        {
            var configFile = new FileInfo(configFileName);
            if (configFile.Exists)
            {
                factory.AddLog4Net();
            }

            _factory = factory;
        }

        private static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new LoggerFactory();
                    ConfigureLogger(_factory);
                }

                return _factory;
            }
            set
            {
                _factory = value;
            }
        }

        public static ILogger<T> GetLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        public static ILogger GetLogger(Type type)
        {
            return LoggerFactory.CreateLogger(type);
        }
    }
}
