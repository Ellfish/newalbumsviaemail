using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Subscribers;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Utils;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Requests.Subscriber;
using NewAlbums.Web.Responses.Common;

namespace NewAlbums.Web.Controllers
{
    [Route("api/[controller]")]
    [RequestValidation]
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberAppService _subscriberAppService;

        public SubscriberController(
            ISubscriberAppService subscriberAppService)
        {
            _subscriberAppService = subscriberAppService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var verifyOutput = await _subscriberAppService.CheckEmailVerification(new CheckEmailVerificationInput
            {
                EmailAddress = request.EmailAddress,
                VerifyCode = request.VerifyCode
            });

            if (verifyOutput.HasError)
                return StatusCode(500, new ApiResponse(500, verifyOutput.ErrorMessage));

            var updateOut = await _subscriberAppService.Update(new UpdateSubscriberInput
            {
                EmailAddressVerified = true
            });

            if (updateOut.HasError)
                return StatusCode(500, new ApiResponse(500, updateOut.ErrorMessage));

            return Ok(new ApiOkResponse(true));
        }
    }
}