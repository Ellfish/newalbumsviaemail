using Microsoft.Extensions.Logging;
using NewAlbums.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NewAlbums
{
    public abstract class BaseAppService
    {
        protected ILogger Logger { get; private set; }

        protected BaseAppService()
        {
            //Resolve from singleton so we don't need to pass ILogger into constructor of 
            //every app service derived from BaseAppService 
            Logger = NewAlbumsLogging.GetLogger(GetType());
        }
    }
}
