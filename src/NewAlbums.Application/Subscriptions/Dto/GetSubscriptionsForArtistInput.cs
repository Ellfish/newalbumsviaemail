using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class GetSubscriptionsForArtistInput
    {
        [Required]
        public long ArtistId { get; set; }
    }
}
