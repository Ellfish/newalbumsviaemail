using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Albums.Dto
{
    public class CreateNewAlbumsInput
    {
        public IList<AlbumDto> Albums { get; set; }

        public CreateNewAlbumsInput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
