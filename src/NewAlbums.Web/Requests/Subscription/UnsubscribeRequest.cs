using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewAlbums.Web.Requests.Subscription
{
    public class UnsubscribeRequest
    {
        [Required]
        public string UnsubscribeToken { get; set; }

        public long? ArtistId { get; set; }
    }
}
