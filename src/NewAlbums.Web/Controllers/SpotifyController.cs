using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewAlbums.Spotify;
using NewAlbums.Spotify.Dto;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Requests.Spotify;
using NewAlbums.Web.Responses.Common;

namespace NewAlbums.Web.Controllers
{
    [Route("api/[controller]")]
    [RequestValidationAttribute]
    public class SpotifyController : BaseController
    {
        private readonly ISpotifyAppService _spotifyAppService;

        public SpotifyController(ISpotifyAppService spotifyAppService)
        {
            _spotifyAppService = spotifyAppService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FollowedArtists([FromQuery] GetFollowedArtistsRequest model)
        {
            var output = await _spotifyAppService.GetFollowedArtists(new GetFollowedArtistsInput
            {
                AccessToken = model.AccessToken
            });

            if (output.HasError)
                return StatusCode(500, new ApiResponse(500, output.ErrorMessage));

            return Ok(new ApiOkResponse(output.Artists));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeToArtists(SubscribeToArtistsRequest model)
        {
            if (!model.SpotifyArtists.Any())
                return BadRequest(new ApiResponse(400, "Please select at least one artist to subscribe to."));

            var output = await _spotifyAppService.SubscribeToArtists(new SubscribeToArtistsInput
            {
                EmailAddress = model.EmailAddress,
                SpotifyArtists = model.SpotifyArtists
            });

            return Ok(new ApiOkResponse(true));
        }
    }
}