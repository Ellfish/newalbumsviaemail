using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewAlbums.Spotify;
using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Notifications
{
    public class Functions
    {
        private readonly ISpotifyAppService _spotifyAppService;

        public Functions(ISpotifyAppService spotifyAppService)
        {
            _spotifyAppService = spotifyAppService;
        }

        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, ILogger logger)
        {
            logger.LogInformation(message);
        }

        [NoAutomaticTrigger]
        public async Task ProcessNewSpotifyAlbums()
        {
            var newAlbumsOutput = await _spotifyAppService.GetNewAlbums(new GetNewAlbumsInput());


        }
    }
}
