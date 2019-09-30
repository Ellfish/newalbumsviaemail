using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class FilterAlbumsByExistingSubscriptionsInput
    {
        public IList<AlbumDto> Albums { get; set; }

        public FilterAlbumsByExistingSubscriptionsInput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
