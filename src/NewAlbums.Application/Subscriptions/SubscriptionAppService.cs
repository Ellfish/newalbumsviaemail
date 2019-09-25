using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewAlbums.Subscriptions.Dto;

namespace NewAlbums.Subscriptions
{
    public class SubscriptionAppService : BaseAppService, ISubscriptionAppService
    {
        public async Task<SubscribeToArtistsOutput> SubscribeToArtists(SubscribeToArtistsInput input)
        {
            throw new NotImplementedException();
        }
    }
}
