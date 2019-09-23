using Microsoft.Extensions.Logging;
using NewAlbums.Spotify.Dto;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Spotify
{
    public class SpotifyAppService : BaseAppService, ISpotifyAppService
    {
        private const int MAX_LIMIT_GET_FOLLOWED_ARTISTS = 50;

        public SpotifyAppService()
        {
            
        }

        public async Task<GetFollowedArtistsOutput> GetFollowedArtists(GetFollowedArtistsInput input)
        {
            var api = new SpotifyWebAPI
            {
                AccessToken = input.AccessToken,
                TokenType = "Bearer",
                UseAuth = true
            };

            var response = await api.GetFollowedArtistsAsync(FollowType.Artist, limit: MAX_LIMIT_GET_FOLLOWED_ARTISTS);
            if (response.HasError())
            {
                Logger.LogError("Error status: {0}, message: {1}", response.Error.Status, response.Error.Message);

                return new GetFollowedArtistsOutput
                {
                    ErrorMessage = response.Error.Message
                };
            }

            var followedArtists = new List<SpotifyArtistDto>();
            
            foreach (var artist in response.Artists.Items)
            {
                followedArtists.Add(new SpotifyArtistDto
                {
                    Id = artist.Id,
                    Name = artist.Name
                });
            }

            return new GetFollowedArtistsOutput
            {
                Artists = followedArtists
            };
        }
    }
}
