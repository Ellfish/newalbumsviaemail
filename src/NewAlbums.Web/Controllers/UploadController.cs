using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Responses.Common;

namespace NewAlbums.Web.Controllers
{
    [Route("api/[controller]")]
    [RequestValidation]
    public class UploadController : BaseController
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
            await Task.Delay(5000);

            var errors = new List<string>
            {
                "Your music collection is far too niche, refusing to work.",
                "No Nickelback found in collection, cannot continue.",
                "Who the fuck buys records these days anyway."
            };

            int index = new Random().Next(0, errors.Count);

            return StatusCode(500, new ApiResponse(500, errors[index]));
        }
    }
}