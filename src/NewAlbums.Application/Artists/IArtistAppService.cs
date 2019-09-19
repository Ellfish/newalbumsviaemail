using NewAlbums.Artists.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Artists
{
    public interface IArtistAppService
    {
        IList<ArtistListDto> GetAll();
    }
}
