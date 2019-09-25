using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewAlbums.Artists;
using NewAlbums.Artists.Dto;
using NewAlbums.Subscribers;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Subscriptions;
using NewAlbums.Subscriptions.Dto;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Requests.Subscription;
using NewAlbums.Web.Responses.Common;

namespace NewAlbums.Web.Controllers
{
    [Route("api/[controller]")]
    [RequestValidation]
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriberAppService _subscriberAppService;
        private readonly IArtistAppService _artistAppService;
        private readonly ISubscriptionAppService _subscriptionAppService;

        public SubscriptionController(
            ISubscriberAppService subscriberAppService,
            IArtistAppService artistAppService,
            ISubscriptionAppService subscriptionAppService)
        {
            _subscriberAppService = subscriberAppService;
            _artistAppService = artistAppService;
            _subscriptionAppService = subscriptionAppService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeToArtists(SubscribeToArtistsRequest model)
        {
            if (!model.SpotifyArtists.Any())
                return BadRequest(new ApiResponse(400, "Please select at least one artist to subscribe to."));

            var subscriberOutput = await _subscriberAppService.GetOrCreate(new GetOrCreateSubscriberInput
            {
                EmailAddress = model.EmailAddress
            });

            if (subscriberOutput.HasError)
                return StatusCode(500, new ApiResponse(500, subscriberOutput.ErrorMessage));

            var artistsOutput = await _artistAppService.GetOrCreateMany(new GetOrCreateManyInput
            {
                Artists = model.SpotifyArtists
            });

            /*
            var output = await _subscriptionAppService.SubscribeToArtists(new SubscribeToArtistsInput
            {
                EmailAddress = model.EmailAddress,
                SpotifyArtists = model.SpotifyArtists
            });
            */

            return Ok(new ApiOkResponse(true));
        }
    }
}