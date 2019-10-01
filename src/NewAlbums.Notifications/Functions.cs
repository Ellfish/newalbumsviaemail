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

        public Functions(
            ISpotifyAppService spotifyAppService,
            ISubscriptionAppService subscriptionAppService,
            IAlbumAppService albumAppService,
            ISubscriberAppService subscriberAppService
            )
        {
            _spotifyAppService = spotifyAppService;
            _subscriptionAppService = subscriptionAppService;
            _albumAppService = albumAppService;
            _subscriberAppService = subscriberAppService;
        }

        /// <summary>
        /// Get all artists, foreach artist get all albums, for any new albums notify subscribers to that artist.
        /// It would have been better to use 
        /// </summary>
        [NoAutomaticTrigger]
        public async Task ProcessNewSpotifyAlbums(ILogger logger)
        {
            //Get 



            var allNewAlbumsOutput = await _spotifyAppService.GetNewAlbums(new GetNewAlbumsInput());

            if (allNewAlbumsOutput.HasError)
            {
                logger.LogError(allNewAlbumsOutput.ErrorMessage);
                return;
            }
            
            if (!allNewAlbumsOutput.Albums.Any())
            {
                logger.LogInformation("No new albums found from Spotify.");
                return;
            }

            //Reduce albums to only those with subscriptions to their artists
            var filteredAlbumsOutput = await _subscriptionAppService.FilterAlbumsByExistingSubscriptions(new FilterAlbumsByExistingSubscriptionsInput
            {
                Albums = allNewAlbumsOutput.Albums
            });

            if (filteredAlbumsOutput.HasError)
            {
                logger.LogError(filteredAlbumsOutput.ErrorMessage);
                return;
            }

            if (!filteredAlbumsOutput.Albums.Any())
            {
                logger.LogInformation("No new albums from Spotify match any saved artists.");
                return;
            }

            //Reduce albums further to only those that haven't already been notified about
            var albumsToNotifyOutput = await _albumAppService.CreateNewAlbums(new CreateNewAlbumsInput
            {
                Albums = filteredAlbumsOutput.Albums
            });

            if (albumsToNotifyOutput.HasError)
            {
                logger.LogError(albumsToNotifyOutput.ErrorMessage);
                return;
            }

            if (!albumsToNotifyOutput.NewAlbums.Any())
            {
                logger.LogInformation("No new albums that haven't already been notified.");
                return;
            }

            var notifyOutput = await _subscriberAppService.NotifySubscribers(new NotifySubscribersInput
            {
                Albums = albumsToNotifyOutput.NewAlbums
            });

            if (notifyOutput.HasError)
            {
                logger.LogError(notifyOutput.ErrorMessage);
                return;
            }
        }
    }
}
