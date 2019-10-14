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
        /// Enforced by us. Max number of artists we want to return for a user, to avoid hitting the Spotify API
        /// too hard and to reduce load time of the request
        /// </summary>
        public const int MaxLimitTotalFollowedArtists = MaxLimitGetFollowedArtists * 200;

        /// <summary>
        /// Enforced by Spotify API, no paging
        /// </summary>
        public const int MaxLimitGetTopArtists = 50;

        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        public const int MaxLimitGetSearchItems = 50;

        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        public const int MaxLimitTotalSearchItems = 10000;

        /// <summary>
        /// Enforced by Spotify API
        /// </summary>
        public const int MaxLimitGetArtistAlbums = 50;

        /// <summary>
        /// Enforced by us. 500 albums should be the absolute maximum that an artist might have
        /// </summary>
        public const int MaxLimitTotalArtistAlbums = MaxLimitGetArtistAlbums * 10;

        /// <summary>
        /// Only show albums etc that are available to the Australian market
        /// </summary>
        public const string MarketToUse = "AU";
    }
}
