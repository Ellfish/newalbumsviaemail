using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums.Dto;
using NewAlbums.Artists.Dto;
using NewAlbums.Configuration;
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

            //Keep requesting MaxLimitGetFollowedArtists until we get all followed artists, or reach MaxLimitTotalFollowedArtists
            int i = SpotifyConsts.MaxLimitTotalFollowedArtists / SpotifyConsts.MaxLimitGetFollowedArtists;
            string afterArtistId = "";

            while (i > 0)
            {
                var response = await api.GetFollowedArtistsAsync(FollowType.Artist, limit: SpotifyConsts.MaxLimitGetFollowedArtists, after: afterArtistId);

                if (response.HasError())
                {
                    Logger.LogError("Error status: {0}, message: {1}", response.Error.Status, response.Error.Message);

                    if (await HandleRateLimitingError(response))
                        continue;

                    return new GetFollowedArtistsOutput
                    {
                        ErrorMessage = response.Error.Message
                    };
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
            var albums = new List<AlbumDto>();

            var api = await GetApiWithClientCredentials();

            int max = SpotifyConsts.MaxLimitTotalSearchItems / SpotifyConsts.MaxLimitGetSearchItems;
            int i = 0;
            while (i < max)
            {
                int offset = i * SpotifyConsts.MaxLimitGetSearchItems;
                //tag:new retrieves only albums released in the last two weeks
                var searchResponse = await api.SearchItemsAsync("tag:new", SearchType.Album, SpotifyConsts.MaxLimitGetSearchItems, offset);

                if (searchResponse.HasError())
                {
                    Logger.LogError("Error status: {0}, message: {1}", searchResponse.Error.Status, searchResponse.Error.Message);

                    if (await HandleRateLimitingError(searchResponse))
                        continue;

                    return new GetNewAlbumsOutput
                    {
                        ErrorMessage = searchResponse.Error.Message
                    };
                }

                if (searchResponse.Albums.Items == null || searchResponse.Albums.Items.Count == 0)
                    break;

                albums.AddRange(
                    searchResponse.Albums.Items.Select(albumItem => new AlbumDto
                    {
                        SpotifyId = albumItem.Id,
                        Name = albumItem.Name,
                        Artists = albumItem.Artists.Select(artistItem => new ArtistDto
                        {
                            SpotifyId = artistItem.Id,
                            Name = artistItem.Name
                        }).ToList()
                    })
                );

                //Assume that if less than the limit are returned, that we're on the last page
                if (searchResponse.Albums.Total < SpotifyConsts.MaxLimitGetSearchItems)
                    break;

                i++;
            }

            return new GetNewAlbumsOutput
            {
                Albums = albums
            };
        }

        /// <summary>
        /// For Spotify API calls that only require client authorization, not tied to a specific user
        /// See: https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow
        /// </summary>
        private async Task<SpotifyWebAPI> GetApiWithClientCredentials()
        {
            string clientId = _configuration.GetValue<string>(AppSettingKeys.Spotify.ClientId);
            string clientSecret = _configuration.GetValue<string>(AppSettingKeys.Spotify.ClientSecret);

            var auth = new CredentialsAuth(clientId, clientSecret);

            var accessToken = await auth.GetToken();

            if (accessToken.HasError())
            {
                Logger.LogError("ClientId: {0}, Error: {1}, ErrorDescription: {2}", clientId, accessToken.Error, accessToken.ErrorDescription);
                throw new Exception("Error requesting Spotify access token: " + accessToken.Error);
            }

            return new SpotifyWebAPI
            {
                AccessToken = accessToken.AccessToken,
                TokenType = accessToken.TokenType,
                UseAuth = true
            };
        }

        /// <summary>
        /// Returns true if there was a rate limit error and we waited long enough to try again.
        /// https://developer.spotify.com/documentation/web-api/#rate-limiting
        /// </summary>
        private async Task<bool> HandleRateLimitingError(BasicModel response)
        {
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

                        return true;
                    }
                }
            }

            return false;
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
