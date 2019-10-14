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

        /// <summary>
        /// If true, we return the user's top 50 artists with Selected = true
        /// </summary>
        public bool PreselectTopArtists { get; set; }
    }
}
