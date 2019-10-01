using NewAlbums.Albums;
using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Artists
{
    public class Artist : CreationAuditedEntity<long>
    {
        [MaxLength(255)]
        public virtual string Name { get; set; }

        [MaxLength(30)]
        public virtual string SpotifyId { get; set; }

        public virtual ICollection<ArtistAlbum> Albums { get; set; }
    }
}
