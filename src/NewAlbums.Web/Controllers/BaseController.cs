using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewAlbums.Logging;

namespace NewAlbums.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ILogger Logger { get; private set; }

        protected BaseController()
        {
            //Resolve from singleton so we don't need to pass ILogger into constructor of 
            //every controller derived from BaseController 
            Logger = NewAlbumsLogging.GetLogger(GetType());
        }
    }
}