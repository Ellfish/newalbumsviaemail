using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Albums.Dto
{
    public class CreateNewAlbumsOutput : BaseOutput
    {
        public IList<AlbumDto> NewAlbums { get; set; }

        public CreateNewAlbumsOutput()
        {
            NewAlbums = new List<AlbumDto>();
        }
    }
}
