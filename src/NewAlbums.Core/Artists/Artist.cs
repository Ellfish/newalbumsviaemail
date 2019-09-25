using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists
{
    public class Artist : CreationAuditedEntity<long>
    {
        public virtual string Name { get; set; }

        public virtual string SpotifyId { get; set; }
    }
}
