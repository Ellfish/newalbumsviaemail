using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class NotifySubscribersInput
    {
        public IList<AlbumDto> Albums { get; set; }

        public NotifySubscribersInput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
