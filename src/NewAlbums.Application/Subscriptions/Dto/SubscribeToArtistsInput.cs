using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
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
