using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class GetNewAlbumsOutput : BaseOutput
    {
        public IList<AlbumDto> Albums { get; set; }

        public GetNewAlbumsOutput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
