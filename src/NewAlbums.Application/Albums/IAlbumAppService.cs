using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Albums
{
    public interface IAlbumAppService
    {
        Task<CreateAlbumsOutput> CreateAlbums(CreateAlbumsInput input);
    }
}
