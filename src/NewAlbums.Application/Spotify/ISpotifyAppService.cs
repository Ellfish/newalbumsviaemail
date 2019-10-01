using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Spotify
{
    public interface ISpotifyAppService
    {
        Task<GetFollowedArtistsOutput> GetFollowedArtists(GetFollowedArtistsInput input);

        Task<GetAlbumsForArtistOutput> GetAlbumsForArtist(GetAlbumsForArtistInput input);

        //Not used
        //Task<GetNewAlbumsOutput> GetNewAlbums(GetNewAlbumsInput input);
    }
}
