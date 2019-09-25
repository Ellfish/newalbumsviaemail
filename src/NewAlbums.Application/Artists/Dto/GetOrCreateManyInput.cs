using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class GetOrCreateManyInput
    {
        public IList<SpotifyArtistDto> Artists { get; set; }

        public GetOrCreateManyInput()
        {
            Artists = new List<SpotifyArtistDto>();
        }
    }
}
