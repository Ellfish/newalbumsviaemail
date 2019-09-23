using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class GetFollowedArtistsOutput : BaseOutput
    {
        public IList<SpotifyArtistDto> Artists { get; set; }

        public GetFollowedArtistsOutput()
        {
            Artists = new List<SpotifyArtistDto>();
        }
    }
}
