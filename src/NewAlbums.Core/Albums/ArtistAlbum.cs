using NewAlbums.Artists;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewAlbums.Albums
{
    /// <summary>
    /// Many to many relationship between Artists and Albums. Configured by Fluent API in NewAlbumsDbContext.
    /// </summary>
    public class ArtistAlbum
    {
        public virtual long ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }

        public virtual long AlbumId { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }
    }
}
