using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums
{
    public abstract class BaseAppService
    {
        /// <summary>
        /// TODO: property injected
        /// </summary>
        public ILogger Logger { protected get; set; }
    }
}
