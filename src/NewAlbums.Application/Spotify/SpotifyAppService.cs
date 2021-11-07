using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums.Dto;
using NewAlbums.Configuration;
using NewAlbums.Spotify.Dto;
using SpotifyAPI.Web;

namespace NewAlbums.Spotify
{
    public class SpotifyAppService : BaseAppService, ISpotifyAppService
    {
        private readonly IConfiguration _configuration;

        private SpotifyClient _apiWithClientCredentials;
        private ClientCredentialsTokenResponse _apiAccessToken;

        public SpotifyAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GetFollowedArtistsOutput> GetFollowedArtists(GetFollowedArtistsInput input)
        {
            Logger.LogInformation("Getting followed artists...");

            try
            {
                //Initialise the SpotifyClient with the access token provided by the user authenticating with Spotify
                var api = new SpotifyClient(input.AccessToken);

                var followedArtists = new List<SpotifyArtistDto>();

                //Also get top artists if requested
                var topArtistIds = new List<string>();
                if (input.PreselectTopArtists)
                {
                    topArtistIds = await GetTopArtistIds(input.AccessToken);
                }

                //Keep requesting MaxLimitGetFollowedArtists until we get all followed artists, or reach MaxLimitTotalFollowedArtists
                int i = SpotifyConsts.MaxLimitTotalFollowedArtists / SpotifyConsts.MaxLimitGetFollowedArtists;
                string afterArtistId = null;

                while (i > 0)
                {
                    try
                    {
                        var response = await api.Follow.OfCurrentUser(new FollowOfCurrentUserRequest
                        {
                            TypeParam = FollowOfCurrentUserRequest.Type.Artist,
                            Limit = SpotifyConsts.MaxLimitGetFollowedArtists,
                            After = afterArtistId
                        });

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

                        if (String.IsNullOrEmpty(response.Artists.Next))
                            break;

                        afterArtistId = response.Artists.Items[response.Artists.Items.Count - 1].Id;

                        i--;

                    }
                    catch (APITooManyRequestsException ex)
                    {
                        await HandleRateLimitingError(ex);
                    }
                    catch (APIException ex)
                    {
                        Logger.LogError("Error message: {0}", ex.Message);

                        return new GetFollowedArtistsOutput
                        {
                            ErrorMessage = ex.Message
                        };
                    }
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

        /// <summary>
        /// Attempt to get the user's email address. If this fails for any reason, it's not critical - it just
        /// means we have to validate the email address before sending any other emails
        /// </summary>
        public async Task<GetUserEmailOutput> GetUserEmail(GetUserEmailInput input)
        {
            Logger.LogInformation("Getting user email...");

            try
            {
                //Initialise the SpotifyClient with the access token provided by the user authenticating with Spotify
                var api = new SpotifyClient(input.AccessToken);

                var response = await api.UserProfile.Current();

                //Don't actually log their email address, for privacy/security reasons
                Logger.LogInformation("Returning user email...");

                return new GetUserEmailOutput
                {
                    EmailAddress = response.Email
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new GetUserEmailOutput
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
                //Initialise the SpotifyClient with the access token provided by the user authenticating with Spotify
                var api = new SpotifyClient(accessToken);

                var topArtistIds = new List<string>();

                var response = await api.Personalization.GetTopArtists(new PersonalizationTopRequest
                {
                    TimeRangeParam = PersonalizationTopRequest.TimeRange.LongTerm,
                    Limit = SpotifyConsts.MaxLimitGetTopArtists
                });

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
                    if (_apiWithClientCredentials == null || _apiAccessToken.IsExpired)
                    {
                        var apiOutput = await GetApiWithClientCredentials();
                        _apiWithClientCredentials = apiOutput.Api;
                        _apiAccessToken = apiOutput.TokenResponse;
                    }

                    int offset = i * SpotifyConsts.MaxLimitGetArtistAlbums;

                    try
                    {
                        var searchResponse = await _apiWithClientCredentials.Artists.GetAlbums(input.SpotifyArtistId, new ArtistsAlbumsRequest
                        {
                            Offset = offset,
                            Market = SpotifyConsts.MarketToUse,
                            IncludeGroupsParam = ArtistsAlbumsRequest.IncludeGroups.Album
                        });

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

                        if (String.IsNullOrEmpty(searchResponse.Next))
                            break;

                        i++;
                    }
                    catch (APITooManyRequestsException ex)
                    {
                        await HandleRateLimitingError(ex);

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
            var albums = new List<AlbumDto>();

            int max = SpotifyConsts.MaxLimitTotalSearchItems / SpotifyConsts.MaxLimitGetSearchItems;
            int i = 0;
            while (i < max)
            {
                //Allows us to only request a new API access token for the first time, or when the existing one expires
                if (_apiWithClientCredentials == null || _apiAccessToken.IsExpired)
                {
                    var apiOutput = await GetApiWithClientCredentials();
                    _apiWithClientCredentials = apiOutput.Api;
                    _apiAccessToken = apiOutput.TokenResponse;
                }

                int offset = i * SpotifyConsts.MaxLimitGetSearchItems;

                //tag:new retrieves only albums released in the last two weeks
                var search = _apiWithClientCredentials.Search;
                var searchRequest = new SearchRequest(SearchRequest.Types.Album, "tag:new");
                searchRequest.Limit = SpotifyConsts.MaxLimitGetSearchItems;
                searchRequest.Offset = offset;
                searchRequest.Market = SpotifyConsts.MarketToUse;

                try
                {
                    var searchResponse = await search.Item(searchRequest);

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

                    if (String.IsNullOrEmpty(searchResponse.Albums.Next))
                        break;

                    i++;
                }
                catch (APITooManyRequestsException ex)
                {
                    await HandleRateLimitingError(ex);
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

            return new GetNewAlbumsOutput
            {
                Albums = albums
            };
        }

        /// <summary>
        /// For Spotify API calls that only require client authorization, not tied to a specific user
        /// See: https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow
        /// </summary>
        private async Task<GetSpotifyApiOutput> GetApiWithClientCredentials()
        {
            string clientId = _configuration.GetValue<string>(AppSettingKeys.Spotify.ClientId);
            string clientSecret = _configuration.GetValue<string>(AppSettingKeys.Spotify.ClientSecret);
            var config = SpotifyClientConfig.CreateDefault();

            try
            {
                var request = new ClientCredentialsRequest(clientId, clientSecret);
                var response = await new OAuthClient(config).RequestToken(request);

                return new GetSpotifyApiOutput
                {
                    Api = new SpotifyClient(response.AccessToken, response.TokenType),
                    TokenResponse = response
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("ClientId: {0}, Message: {1}", clientId, ex.Message);
                throw new Exception("Error requesting Spotify access token: " + ex.Message);
            }
        }

        /// <summary>
        /// https://developer.spotify.com/documentation/web-api/#rate-limiting
        /// </summary>
        private async Task HandleRateLimitingError(APITooManyRequestsException ex)
        {
            TimeSpan retryAfter = ex.RetryAfter;

            Logger.LogInformation("Received TooManyRequests response with Retry-After: {0}", retryAfter.TotalSeconds);

            // Handle rate limiting: https://developer.spotify.com/documentation/web-api/#rate-limiting
            // Safest to add one second here, see comments for: https://stackoverflow.com/a/30557896
            await Task.Delay((int) (retryAfter.TotalSeconds + 1) * 1000);
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
