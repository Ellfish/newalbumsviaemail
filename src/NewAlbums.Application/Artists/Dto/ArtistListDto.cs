using GenericServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class ArtistListDto : ILinkToEntity<Artist>
    {
        public string Name { get; set; }
    }
}
