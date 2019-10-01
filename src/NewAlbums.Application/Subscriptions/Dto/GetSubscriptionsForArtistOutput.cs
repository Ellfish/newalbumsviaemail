using NewAlbums.Albums.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class GetSubscriptionsForArtistOutput : BaseOutput
    {
        public IList<SubscriptionDto> Subscriptions { get; set; }

        public GetSubscriptionsForArtistOutput()
        {
            Subscriptions = new List<SubscriptionDto>();
        }
    }
}
