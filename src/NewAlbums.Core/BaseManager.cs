using Microsoft.Extensions.Logging;
using NewAlbums.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums
{
    public abstract class BaseManager
    {
        protected ILogger Logger { get; private set; }

        protected BaseManager()
        {
            //Resolve from singleton so we don't need to pass ILogger into constructor of 
            //every app service derived from BaseAppService 
            Logger = NewAlbumsLogging.GetLogger(GetType());
        }
    }
}
