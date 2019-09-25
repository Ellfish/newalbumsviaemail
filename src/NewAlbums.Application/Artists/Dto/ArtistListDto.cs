using GenericServices;
using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class ArtistListDto : CreationAuditedEntityDto<long>, ILinkToEntity<Artist>
    {
        public string Name { get; set; }
    }
}
