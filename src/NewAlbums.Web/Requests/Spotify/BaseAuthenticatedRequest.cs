﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewAlbums.Web.Requests.Spotify
{
    public abstract class BaseAuthenticatedRequest
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
