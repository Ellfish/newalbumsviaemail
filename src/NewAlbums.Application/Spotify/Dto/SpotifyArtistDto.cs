﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class SpotifyArtistDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public SpotifyImageDto Image { get; set; }
    }
}
