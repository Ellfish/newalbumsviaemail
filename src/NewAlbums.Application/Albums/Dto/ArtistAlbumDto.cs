using GenericServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Albums.Dto
{
    public class ArtistAlbumDto : ILinkToEntity<ArtistAlbum>
    {
        public long ArtistId { get; set; }

        public long AlbumId { get; set; }

        public AlbumDto Album { get; set; }
    }
}
