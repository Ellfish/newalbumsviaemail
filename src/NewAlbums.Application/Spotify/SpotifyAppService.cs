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

        private SpotifyWebAPI _apiWithClientCredentials = null;
        private Token _apiAccessToken = null;

        public SpotifyAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GetFollowedArtistsOutput> GetFollowedArtists(GetFollowedArtistsInput input)
        {
            Logger.LogInformation("Getting followed artists...");

            try
            {
                //Initialise the SpotifyWebAPI with the access token provided by the user authenticating with Spotify
                var api = new SpotifyWebAPI
                {
                    AccessToken = input.AccessToken,
                    TokenType = "Bearer",
                    UseAuth = true
                };

                var followedArtists = new List<SpotifyArtistDto>();
                
                //Also get top artists if requested
                var topArtistIds = new List<string>();
                if (input.PreselectTopArtists)
                {
                    topArtistIds = await GetTopArtistIds(input.AccessToken);
                }

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
                            Image = GetImage(artistItem.Images, 100),
                            Selected = topArtistIds.Contains(artistItem.Id)
                        })
                    );

                    //Uncomment to only return a manageable amount of artists when debugging/testing
                    //if (DebugHelper.IsDebug)
                    //{
                    //    followedArtists = followedArtists.Take(20).ToList();
                    //    break;
                    //}

                    if (!response.Artists.HasNext())
                        break;

                    afterArtistId = response.Artists.Items[response.Artists.Items.Count - 1].Id;

                    i--;
                }

                Logger.LogInformation("Returning {0} followed artists.", followedArtists.Count);

                var followedArtistIds = followedArtists.Select(a => a.Id).ToList();
                var ids = topArtistIds.Where(id => !followedArtistIds.Contains(id));


                return new GetFollowedArtistsOutput
                {
                    Artists = followedArtists.OrderBy(a => a.Name).ToList(),
                    ErrorMessage = !followedArtists.Any() ? "You're not currently following any artists." : null
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new GetFollowedArtistsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        protected async Task<List<string>> GetTopArtistIds(string accessToken)
        {
            Logger.LogInformation("Getting top artists...");

            try
            {
                //Initialise the SpotifyWebAPI with the access token provided by the user authenticating with Spotify
                var api = new SpotifyWebAPI
                {
                    AccessToken = accessToken,
                    TokenType = "Bearer",
                    UseAuth = true
                };

                var topArtistIds = new List<string>();

                var response = await api.GetUsersTopArtistsAsync(TimeRangeType.LongTerm, limit: SpotifyConsts.MaxLimitGetTopArtists);

                if (response.HasError())
                {
                    Logger.LogError("Error status: {0}, message: {1}", response.Error.Status, response.Error.Message);
                    //Not an essential method, just return empty
                    return new List<string>();
                }

                topArtistIds.AddRange(response.Items.Select(artistItem => artistItem.Id));

                Logger.LogInformation("Returning {0} top artists.", topArtistIds.Count);
                return topArtistIds;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                //Not an essential method, just return empty
                return new List<string>();
            }
        }

        public async Task<GetAlbumsForArtistOutput> GetAlbumsForArtist(GetAlbumsForArtistInput input)
        {
            if (String.IsNullOrWhiteSpace(input.SpotifyArtistId))
                throw new ArgumentException("SpotifyArtistId must be a valid Spotify Id", "SpotifyArtistId");

            Logger.LogInformation("Getting albums for SpotifyArtistId: {0}", input.SpotifyArtistId);

            try
            {
                var albums = new List<AlbumDto>();

                int max = SpotifyConsts.MaxLimitTotalArtistAlbums / SpotifyConsts.MaxLimitGetArtistAlbums;
                int i = 0;
                while (i < max)
                {
                    //Allows us to only request a new API access token for the first time, or when the existing one expires
                    if (_apiWithClientCredentials == null || _apiAccessToken.IsExpired())
                    {
                        var apiOutput = await GetApiWithClientCredentials();
                        _apiWithClientCredentials = apiOutput.Api;
                        _apiAccessToken = apiOutput.Token;
                    }

                    int offset = i * SpotifyConsts.MaxLimitGetArtistAlbums;
                    
                    var searchResponse = await _apiWithClientCredentials.GetArtistsAlbumsAsync(input.SpotifyArtistId, AlbumType.Album, 
                        SpotifyConsts.MaxLimitGetArtistAlbums, offset, SpotifyConsts.MarketToUse);

                    if (searchResponse.HasError())
                    {
                        Logger.LogError("Error status: {0}, message: {1}", searchResponse.Error.Status, searchResponse.Error.Message);

                        if (await HandleRateLimitingError(searchResponse))
                            continue;

                        return new GetAlbumsForArtistOutput
                        {
                            ErrorMessage = searchResponse.Error.Message
                        };
                    }

                    //Assume there are no more if this is true
                    if (searchResponse.Items == null || searchResponse.Items.Count == 0)
                        break;

                    albums.AddRange(
                        searchResponse.Items.Select(albumItem => new AlbumDto
                        {
                            SpotifyId = albumItem.Id,
                            Name = albumItem.Name,
                            Image = GetImage(albumItem.Images, 200),
                            ReleaseDate = albumItem.ReleaseDate
                        })
                    );

                    if (!searchResponse.HasNextPage())
                        break;

                    i++;
                }

                Logger.LogInformation("Returning {0} albums for SpotifyArtistId: {1}", albums.Count, input.SpotifyArtistId);

                return new GetAlbumsForArtistOutput
                {
                    Albums = albums
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new GetAlbumsForArtistOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Can't use this because Spotify enforces a limit of 10,000 items, even though there may be 10 times that number of
        /// tag:new album results. Can't filter futher by AlbumType or release date either. https://github.com/spotify/web-api/issues/862
        /// </summary>
        public async Task<GetNewAlbumsOutput> GetNewAlbums(GetNewAlbumsInput input)
        {
            try
            {
                var albums = new List<AlbumDto>();

                int max = SpotifyConsts.MaxLimitTotalSearchItems / SpotifyConsts.MaxLimitGetSearchItems;
                int i = 0;
                while (i < max)
                {
                    //Allows us to only request a new API access token for the first time, or when the existing one expires
                    if (_apiWithClientCredentials == null || _apiAccessToken.IsExpired())
                    {
                        var apiOutput = await GetApiWithClientCredentials();
                        _apiWithClientCredentials = apiOutput.Api;
                        _apiAccessToken = apiOutput.Token;
                    }

                    int offset = i * SpotifyConsts.MaxLimitGetSearchItems;
                    //tag:new retrieves only albums released in the last two weeks
                    var searchResponse = await _apiWithClientCredentials.SearchItemsAsync("tag:new", SearchType.Album, 
                        SpotifyConsts.MaxLimitGetSearchItems, offset, SpotifyConsts.MarketToUse);

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
                            Image = GetImage(albumItem.Images, 200),
                            ReleaseDate = albumItem.ReleaseDate,
                            //Artists = albumItem.Artists.Select(artistItem => new ArtistDto
                            //{
                            //    SpotifyId = artistItem.Id,
                            //    Name = artistItem.Name
                            //}).ToList()
                        })
                    );

                    if (!searchResponse.Albums.HasNextPage())
                        break;

                    i++;
                }

                return new GetNewAlbumsOutput
                {
                    Albums = albums
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new GetNewAlbumsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// For Spotify API calls that only require client authorization, not tied to a specific user
        /// See: https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow
        /// </summary>
        private async Task<GetSpotifyApiOutput> GetApiWithClientCredentials()
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

            return new GetSpotifyApiOutput
            {
                Api = new SpotifyWebAPI
                {
                    AccessToken = accessToken.AccessToken,
                    TokenType = accessToken.TokenType,
                    UseAuth = true
                },
                Token = accessToken
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

                Logger.LogInformation("Received TooManyRequests response with Retry-After: {0}", retryAfterSeconds);

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
        /// Finds the smallest image over minWidth wide and returns it as a SpotifyImageDto.
        /// If no images meet this criteria, returns a default image (shouldn't ever happen, but makes front-end simpler just in case)
        /// </summary>
        private SpotifyImageDto GetImage(List<Image> apiImages, int minWidth)
        {
            if (apiImages == null || !apiImages.Any())
                return new SpotifyImageDto();

            var spotifyImage = apiImages
                .Where(i => i.Width >= minWidth)
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

            //Default image properties set in constructor
            return new SpotifyImageDto();
        }
    }
}
