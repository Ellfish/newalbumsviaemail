using GenericServices;
using NewAlbums.Albums.Dto;
using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class ArtistDto : CreationAuditedEntityDto<long>, ILinkToEntity<Artist>
    {
        public string Name { get; set; }

        public string SpotifyId { get; set; }

        public IList<ArtistAlbumDto> Albums { get; set; }

        public ArtistDto()
        {
            Albums = new List<ArtistAlbumDto>();
        }
    }
}
