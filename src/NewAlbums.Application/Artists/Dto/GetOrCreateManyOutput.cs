using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class GetOrCreateManyOutput : BaseOutput
    {
        public IList<ArtistDto> Artists { get; set; }

        public GetOrCreateManyOutput()
        {
            Artists = new List<ArtistDto>();
        }
    }
}
