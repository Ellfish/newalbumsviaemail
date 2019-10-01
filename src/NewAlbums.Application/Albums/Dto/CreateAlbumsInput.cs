using NewAlbums.Artists.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Albums.Dto
{
    public class CreateAlbumsInput
    {
        [Required]
        public ArtistDto Artist { get; set; }

        public IList<AlbumDto> Albums { get; set; }

        public CreateAlbumsInput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
