using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewAlbums.Notifications.Dto;
using NewAlbums.Spotify;
using NewAlbums.Spotify.Dto;
using NewAlbums.Subscriptions;
using NewAlbums.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Notifications
{
    public class Functions
    {
        private readonly ISpotifyAppService _spotifyAppService;
        private readonly ISubscriptionAppService _subscriptionAppService;
        private readonly INotificationAppService _notificationAppService;
        private readonly ILogger _logger;

        public Functions(
            ISpotifyAppService spotifyAppService,
            ISubscriptionAppService subscriptionAppService,
            INotificationAppService notificationAppService,
            ILogger logger
            )
        {
            _spotifyAppService = spotifyAppService;
            _subscriptionAppService = subscriptionAppService;
            _notificationAppService = notificationAppService;
            _logger = logger;
        }

        [NoAutomaticTrigger]
        public async Task ProcessNewSpotifyAlbums()
        {
            var newAlbumsOutput = await _spotifyAppService.GetNewAlbums(new GetNewAlbumsInput());

            if (newAlbumsOutput.HasError)
            {
                _logger.LogError(newAlbumsOutput.ErrorMessage);
                return;
            }
            
            if (!newAlbumsOutput.Albums.Any())
            {
                _logger.LogInformation("No new albums found from Spotify.");
                return;
            }

            var filteredAlbumsOutput = await _subscriptionAppService.FilterAlbumsByExistingSubscriptions(new FilterAlbumsByExistingSubscriptionsInput
            {
                Albums = newAlbumsOutput.Albums
            });

            if (filteredAlbumsOutput.HasError)
            {
                _logger.LogError(filteredAlbumsOutput.ErrorMessage);
                return;
            }

            if (!filteredAlbumsOutput.Albums.Any())
            {
                _logger.LogInformation("No new albums from Spotify match any saved artists.");
                return;
            }

            var notifyOutput = await _notificationAppService.NotifySubscribers(new NotifySubscribersInput
            {
                Albums = filteredAlbumsOutput.Albums
            });

            if (notifyOutput.HasError)
            {
                _logger.LogError(notifyOutput.ErrorMessage);
                return;
            }
        }
    }
}
