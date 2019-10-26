using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class UpdateSubscriberInput
    {
        [Required]
        public long Id { get; set; }

        public bool EmailAddressVerified { get; set; }
    }
}
