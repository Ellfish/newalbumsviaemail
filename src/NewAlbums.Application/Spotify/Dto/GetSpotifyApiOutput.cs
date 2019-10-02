using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class GetSpotifyApiOutput
    {
        public SpotifyWebAPI Api { get; set; }

        public Token Token { get; set; }
    }
}
