using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewAlbums.Web.Requests.Subscription
{
    public class SubscribeToArtistsRequest
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public IList<SpotifyArtistDto> SpotifyArtists { get; set; }

        public SubscribeToArtistsRequest()
        {
            SpotifyArtists = new List<SpotifyArtistDto>();
        }
    }
}
