using NewAlbums.Albums.Dto;
using NewAlbums.Artists.Dto;
using NewAlbums.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class NotifySubscribersInput
    {
        [Required]
        public ArtistDto Artist { get; set; }

        [Required]
        public AlbumDto Album { get; set; }

        public IList<SubscriptionDto> Subscriptions { get; set; }

        public NotifySubscribersInput()
        {
            Subscriptions = new List<SubscriptionDto>();
        }
    }
}
