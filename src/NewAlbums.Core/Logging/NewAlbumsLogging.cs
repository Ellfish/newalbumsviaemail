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

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            var configFile = new FileInfo("log4net.config");
            if (configFile.Exists)
            {
                factory.AddLog4Net();
            }
        }

        public static ILoggerFactory LoggerFactory
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
