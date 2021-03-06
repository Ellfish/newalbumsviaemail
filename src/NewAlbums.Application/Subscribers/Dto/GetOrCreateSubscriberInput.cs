﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class GetOrCreateSubscriberInput
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool EmailAddressVerified { get; set; }
    }
}
