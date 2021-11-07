using AutoMapper;
using NewAlbums.Albums;
using NewAlbums.Albums.Dto;
using NewAlbums.Artists;
using NewAlbums.Artists.Dto;

namespace NewAlbums.Notifications.Profiles
{
    public class ArtistAndAlbumProfile : Profile
    {
        public ArtistAndAlbumProfile()
        {
            CreateMap<Artist, ArtistDto>();
            CreateMap<Album, AlbumDto>();
            CreateMap<ArtistAlbum, ArtistAlbumDto>();
        }
    }
}
