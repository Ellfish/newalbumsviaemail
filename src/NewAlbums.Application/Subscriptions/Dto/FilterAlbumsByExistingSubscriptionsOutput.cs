using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class FilterAlbumsByExistingSubscriptionsOutput : BaseOutput
    {
        public IList<AlbumDto> Albums { get; set; }

        public FilterAlbumsByExistingSubscriptionsOutput()
        {
            Albums = new List<AlbumDto>();
        }
    }
}
