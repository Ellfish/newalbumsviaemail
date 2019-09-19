using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericServices;
using NewAlbums.Artists.Dto;

namespace NewAlbums.Artists
{
    public class ArtistAppService : BaseAppService, IArtistAppService
    {
        private readonly ICrudServices _crudServices;

        public ArtistAppService(ICrudServices crudServices)
        {
            _crudServices = crudServices;
        }

        public IList<ArtistListDto> GetAll()
        {
            return _crudServices.ReadManyNoTracked<ArtistListDto>().ToList();
        }
    }
}
