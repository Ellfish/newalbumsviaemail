using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class SubscribeToArtistsInput
    {
        public string EmailAddress { get; set; }

        public IList<SpotifyArtistDto> SpotifyArtists { get; set; }

        public SubscribeToArtistsInput()
        {
            SpotifyArtists = new List<SpotifyArtistDto>();
        }
    }
}
