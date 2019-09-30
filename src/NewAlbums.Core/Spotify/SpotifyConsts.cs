using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Spotify
{
    public static class SpotifyConsts
    {
        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        public const int MaxLimitGetFollowedArtists = 50;

        /// <summary>
        /// Max number of artists we want to return for a user, to avoid hitting the Spotify API
        /// too hard and to reduce load time of the request
        /// </summary>
        public const int MaxLimitTotalFollowedArtists = MaxLimitGetFollowedArtists * 20;

        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        public const int MaxLimitGetSearchItems = 50;

        /// <summary>
        /// Enforced by us to avoid hitting Spotify API too hard
        /// </summary>
        public const int MaxLimitTotalSearchItems = MaxLimitGetSearchItems * 20;
    }
}
