using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class SpotifyImageDto
    {
        public string Url { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public SpotifyImageDto()
        {
            Url = "/images/default-artist-image.png";
            Width = 180;
            Height = 180;
        }
    }
}
