using NewAlbums.Artists.Dto;
using NewAlbums.Spotify.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewAlbums.Albums.Dto
{
    public class AlbumDto
    {
        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        [ReadOnly(true)]
        public string SpotifyUrl { get; set; }

        public IList<ArtistDto> Artists { get; set; }

        public SpotifyImageDto Image { get; set; }

        public AlbumDto()
        {
            Artists = new List<ArtistDto>();
        }
    }
}
