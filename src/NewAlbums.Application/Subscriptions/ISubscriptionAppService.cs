using NewAlbums.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Subscriptions
{
    public interface ISubscriptionAppService
    {
        Task<SubscribeToArtistsOutput> SubscribeToArtists(SubscribeToArtistsInput input);

        Task<GetSubscriptionsForArtistOutput> GetSubscriptionsForArtist(GetSubscriptionsForArtistInput input);

        Task<UnsubscribeFromArtistOutput> UnsubscribeFromArtist(UnsubscribeFromArtistInput input);

        Task<UnsubscribeFromAllOutput> UnsubscribeFromAll(UnsubscribeFromAllInput input);
    }
}
