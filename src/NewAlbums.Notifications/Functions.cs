using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
using NewAlbums.Spotify;
using NewAlbums.Spotify.Dto;
using NewAlbums.Subscribers;
using NewAlbums.Subscribers.Dto;
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
        private readonly IAlbumAppService _albumAppService;
        private readonly ISubscriberAppService _subscriberAppService;
        private readonly ILogger _logger;

        public Functions(
            ISpotifyAppService spotifyAppService,
            ISubscriptionAppService subscriptionAppService,
            IAlbumAppService albumAppService,
            ISubscriberAppService subscriberAppService,
            ILogger logger
            )
        {
            _spotifyAppService = spotifyAppService;
            _subscriptionAppService = subscriptionAppService;
            _albumAppService = albumAppService;
            _subscriberAppService = subscriberAppService;
            _logger = logger;
        }

        [NoAutomaticTrigger]
        public async Task ProcessNewSpotifyAlbums()
        {
            //Get all new albums from Spotify
            var allNewAlbumsOutput = await _spotifyAppService.GetNewAlbums(new GetNewAlbumsInput());

            if (allNewAlbumsOutput.HasError)
            {
                _logger.LogError(allNewAlbumsOutput.ErrorMessage);
                return;
            }
            
            if (!allNewAlbumsOutput.Albums.Any())
            {
                _logger.LogInformation("No new albums found from Spotify.");
                return;
            }

            //Reduce albums to only those with subscriptions to their artists
            var filteredAlbumsOutput = await _subscriptionAppService.FilterAlbumsByExistingSubscriptions(new FilterAlbumsByExistingSubscriptionsInput
            {
                Albums = allNewAlbumsOutput.Albums
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

            //Reduce albums further to only those that haven't already been notified about
            var albumsToNotifyOutput = await _albumAppService.CreateNewAlbums(new CreateNewAlbumsInput
            {
                Albums = filteredAlbumsOutput.Albums
            });

            if (albumsToNotifyOutput.HasError)
            {
                _logger.LogError(albumsToNotifyOutput.ErrorMessage);
                return;
            }

            if (!albumsToNotifyOutput.NewAlbums.Any())
            {
                _logger.LogInformation("No new albums that haven't already been notified.");
                return;
            }

            var notifyOutput = await _subscriberAppService.NotifySubscribers(new NotifySubscribersInput
            {
                Albums = albumsToNotifyOutput.NewAlbums
            });

            if (notifyOutput.HasError)
            {
                _logger.LogError(notifyOutput.ErrorMessage);
                return;
            }
        }
    }
}
