using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Albums
{
    public class Album : CreationAuditedEntity<long>
    {
        [MaxLength(255)]
        public virtual string Name { get; set; }

        [MaxLength(30)]
        public virtual string SpotifyId { get; set; }

        /// <summary>
        /// From Spotify. The date the album was first released, for example "1981-12-15". Depending on the precision, it might be shown as "1981" or "1981-12"
        /// </summary>
        [MaxLength(10)]
        public virtual string ReleaseDate { get; set; }

        public virtual ICollection<ArtistAlbum> Artists { get; set; }

        public string SpotifyUrl
        {
            get
            {
                return $"https://open.spotify.com/album/{SpotifyId}";
            }
        }
    }
}
