using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Spotify.Dto
{
    public class GetFollowedArtistsInput
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
