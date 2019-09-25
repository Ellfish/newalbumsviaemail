using NewAlbums.Artists.Dto;
using NewAlbums.Spotify.Dto;
using NewAlbums.Subscribers.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class SubscribeToArtistsInput
    {
        public SubscriberDto Subscriber { get; set; }

        public IList<ArtistDto> Artists { get; set; }

        public SubscribeToArtistsInput()
        {
            Artists = new List<ArtistDto>();
        }
    }
}
