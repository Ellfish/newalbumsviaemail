using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class UnsubscribeFromArtistInput
    {
        [Required]
        public string UnsubscribeToken { get; set; }

        [Required]
        public long ArtistId { get; set; }
    }
}
