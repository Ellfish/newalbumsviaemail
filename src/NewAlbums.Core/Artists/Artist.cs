using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists
{
    public class Artist : Entity<long>
    {
        public string Name { get; set; }

        public string SpotifyId { get; set; }
    }
}
