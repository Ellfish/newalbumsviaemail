using NewAlbums.Artists.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Artists
{
    public interface IArtistAppService
    {
        Task<GetOrCreateManyOutput> GetOrCreateMany(GetOrCreateManyInput input);

        Task<GetAllArtistsOutput> GetAll(GetAllArtistsInput input);
    }
}
