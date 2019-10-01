using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists.Dto
{
    public class GetAllArtistsOutput : BaseOutput
    {
        public IList<ArtistDto> Artists { get; set; }

        public GetAllArtistsOutput()
        {
            Artists = new List<ArtistDto>();
        }
    }
}
