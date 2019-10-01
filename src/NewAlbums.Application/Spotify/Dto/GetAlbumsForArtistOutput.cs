using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class GetAlbumsForArtistOutput : BaseOutput
    {
        public IList<AlbumDto> Albums { get; set; }

        public GetAlbumsForArtistOutput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
