using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Debugging;
using NewAlbums.Spotify.Dto;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewAlbums.Spotify
{
    public class SpotifyAppService : BaseAppService, ISpotifyAppService
    {
        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        private const int MAX_LIMIT_GET_FOLLOWED_ARTISTS = 50;

        /// <summary>
        /// Max number of artists we want to return for a user, to avoid hitting the Spotify API
        /// too hard and to reduce load time of the request
        /// </summary>
        private const int MAX_FOLLOWED_ARTISTS = 1000;

        private readonly IConfiguration _configuration;

        public SpotifyAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GetFollowedArtistsOutput> GetFollowedArtists(GetFollowedArtistsInput input)
        {
            var api = new SpotifyWebAPI
            {
                AccessToken = input.AccessToken,
                TokenType = "Bearer",
                UseAuth = true
            };

            var followedArtists = new List<SpotifyArtistDto>();

            //Keep requesting MAX_LIMIT_GET_FOLLOWED_ARTISTS until we get all followed artists, or reach MAX_FOLLOWED_ARTISTS
            int i = MAX_FOLLOWED_ARTISTS / MAX_LIMIT_GET_FOLLOWED_ARTISTS;
            string afterArtistId = "";

            while (i > 0)
            {
                var response = await api.GetFollowedArtistsAsync(FollowType.Artist, limit: MAX_LIMIT_GET_FOLLOWED_ARTISTS, after: afterArtistId);
                if (response.HasError())
                {
                    Logger.LogError("Error status: {0}, message: {1}", response.Error.Status, response.Error.Message);

                    //Handle rate limiting: https://developer.spotify.com/documentation/web-api/#rate-limiting
                    if (response.StatusCode() == HttpStatusCode.TooManyRequests)
                    {
                        string retryAfterSeconds = response.Header("Retry-After");
                        if (!String.IsNullOrWhiteSpace(retryAfterSeconds))
                        {
                            int retryAfterSecondsInt = 0;
                            if (int.TryParse(retryAfterSeconds, out retryAfterSecondsInt))
                            {
                                //Safest to add one second here, see comments for: https://stackoverflow.com/a/30557896
                                retryAfterSecondsInt++;
                                await Task.Delay(retryAfterSecondsInt * 1000);

                                continue;
                            }
                        }
                    }
                    else
                    {
                        return new GetFollowedArtistsOutput
                        {
                            ErrorMessage = response.Error.Message
                        };
                    }
                }

                if (response.Artists.Items == null || response.Artists.Items.Count == 0)
                    break;

                followedArtists.AddRange(
                    response.Artists.Items.Select(artistItem => new SpotifyArtistDto
                    {
                        Id = artistItem.Id,
                        Name = artistItem.Name,
                        Image = GetArtistImage(artistItem.Images)
                    })
                );

                //Only return a manageable amount of artists when debugging
                if (DebugHelper.IsDebug)
                {
                    followedArtists = followedArtists.Take(20).ToList();
                    break;
                }

                if (!response.Artists.HasNext())
                    break;

                afterArtistId = response.Artists.Items[response.Artists.Items.Count - 1].Id;

                i--;
            }

            return new GetFollowedArtistsOutput
            {
                Artists = followedArtists.OrderBy(a => a.Name).ToList(),
                ErrorMessage = !followedArtists.Any() ? "You're not currently following any artists." : null
            };
        }

        public async Task<GetNewAlbumsOutput> GetNewAlbums(GetNewAlbumsInput input)
        {
            var test = _configuration.GetValue<string>("AzureWebJobsStorage");


            return new GetNewAlbumsOutput();
        }

        /// <summary>
        /// Finds the smallest image over 100px wide and sets that as the SpotifyArtistDto.Image.
        /// If no images meet this criteria, returns a default image (shouldn't be necessary, but makes front-end simpler)
        /// </summary>
        private SpotifyImageDto GetArtistImage(List<Image> apiImages)
        {
            if (apiImages == null || !apiImages.Any())
                return new SpotifyImageDto();

            var spotifyImage = apiImages
                .Where(i => i.Width >= 100)
                .OrderBy(i => i.Width)
                .FirstOrDefault();

            if (spotifyImage != null)
            {
                return new SpotifyImageDto
                {
                    Url = spotifyImage.Url,
                    Width = spotifyImage.Width,
                    Height = spotifyImage.Height
                };
            }

            return new SpotifyImageDto();
        }
    }
}
