using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.Extensions.Logging;
using NewAlbums.Subscriptions.Dto;

namespace NewAlbums.Subscriptions
{
    public class SubscriptionAppService : BaseAppService, ISubscriptionAppService
    {
        private readonly ICrudServicesAsync _crudServices;

        public SubscriptionAppService(ICrudServicesAsync crudServices)
        {
            _crudServices = crudServices;
        }

        public async Task<SubscribeToArtistsOutput> SubscribeToArtists(SubscribeToArtistsInput input)
        {
            if (input.Subscriber == null)
                throw new ArgumentException("Subscriber must not be null", "Subscriber");

            if (!input.Artists.Any())
                throw new ArgumentException("Artists must contain at least one artist", "Artists");

            try
            {
                foreach (var artist in input.Artists)
                {
                    //Don't use _crudServices.CreateAndSaveAsync, because we only want to call Save once for performance reasons
                    _crudServices.Context.Add(new Subscription
                    {
                        ArtistId = artist.Id,
                        SubscriberId = input.Subscriber.Id
                    });
                }

                await _crudServices.Context.SaveChangesAsync();

                return new SubscribeToArtistsOutput
                {
                    ErrorMessage = _crudServices.IsValid ? null : _crudServices.GetAllErrors()
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "SubscriberId: " + input.Subscriber);
                return new SubscribeToArtistsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
