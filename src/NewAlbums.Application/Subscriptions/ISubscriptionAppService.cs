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
    }
}
