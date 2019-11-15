using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
using NewAlbums.Artists;
using NewAlbums.Artists.Dto;
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
        private readonly IArtistAppService _artistAppService;
        private readonly IAlbumAppService _albumAppService;
        private readonly ISubscriberAppService _subscriberAppService;

        public Functions(
            ISpotifyAppService spotifyAppService,
            ISubscriptionAppService subscriptionAppService,
            IArtistAppService artistAppService,
            IAlbumAppService albumAppService,
            ISubscriberAppService subscriberAppService
            )
        {
            _spotifyAppService = spotifyAppService;
            _subscriptionAppService = subscriptionAppService;
            _artistAppService = artistAppService;
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
            logger.LogInformation("ProcessNewSpotifyAlbums start");

            //Get all saved Artists
            var allArtistsOutput = await _artistAppService.GetAll(new GetAllArtistsInput { IncludeAlbums = true });
            if (allArtistsOutput.HasError)
            {
                logger.LogError(allArtistsOutput.ErrorMessage);
                return;
            }

            logger.LogInformation("Found {0} saved artists", allArtistsOutput.Artists.Count);

            //An album is only considered a new release if its ReleaseDate was in the last 14 days
            DateTime newReleaseCutoff = DateTime.UtcNow.AddDays(-14);

            foreach (var artist in allArtistsOutput.Artists)
            {
                var artistAlbumsOutput = await _spotifyAppService.GetAlbumsForArtist(new GetAlbumsForArtistInput
                {
                    SpotifyArtistId = artist.SpotifyId
                });

                if (artistAlbumsOutput.HasError)
                {
                    logger.LogError(artistAlbumsOutput.ErrorMessage);
                    //Don't let one artist error stop the entire thing from running, skip and move on to the next
                    continue;
                }

                //If the album came out recently and we don't know about it (ie it wasn't under the Artist.Albums collection),
                //then we want to notify about it
                var newReleaseAlbums = artistAlbumsOutput.Albums
                    .Where(a => a.ReleaseDateNormalised >= newReleaseCutoff 
                        && !artist.Albums.Any(al => al.Album.SpotifyId == a.SpotifyId))
                    .ToList();

                if (newReleaseAlbums.Any())
                {
                    logger.LogInformation("Found {0} new release albums for SpotifyArtistId: {1}", newReleaseAlbums.Count, artist.SpotifyId);

                    var subscriptionsOutput = await _subscriptionAppService.GetSubscriptionsForArtist(new GetSubscriptionsForArtistInput
                    {
                        ArtistId = artist.Id
                    });

                    if (subscriptionsOutput.HasError)
                    {
                        logger.LogError(subscriptionsOutput.ErrorMessage);
                        //Don't let one error stop the entire thing from running, skip and move on to the next
                        continue;
                    }

                    if (subscriptionsOutput.Subscriptions.Any())
                    {
                        //Generally there will just be one newReleaseAlbum. Doesn't matter if we send multiple emails per artist
                        //in the rare case that more than one new release came out for them.
                        foreach (var newReleaseAlbum in newReleaseAlbums)
                        {
                            //Send notification for all subscriptions
                            var notifyOutput = await _subscriberAppService.NotifySubscribers(new NotifySubscribersInput
                            {
                                Artist = artist,
                                Album = newReleaseAlbum,
                                Subscriptions = subscriptionsOutput.Subscriptions
                            });

                            if (notifyOutput.HasError)
                            {
                                logger.LogError(notifyOutput.ErrorMessage);
                                //Don't let one error stop the entire thing from running, skip and move on to the next
                                continue;
                            }
                        }
                    }

                    //Save new albums to database
                    var albumsOutput = await _albumAppService.CreateAlbums(new CreateAlbumsInput
                    {
                        Artist = artist,
                        Albums = newReleaseAlbums
                    });

                    if (albumsOutput.HasError)
                    {
                        logger.LogError(albumsOutput.ErrorMessage);
                        //Don't let one error stop the entire thing from running, skip and move on to the next
                        continue;
                    }
                }
            }

            logger.LogInformation("ProcessNewSpotifyAlbums end");
        }
    }
}
