using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAlbums.Subscribers;
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
                //The subscriber may have existing subscriptions
                var existingSubscriptions = await _crudServices.ReadManyNoTracked<Subscription>()
                    .Where(s => s.SubscriberId == input.Subscriber.Id)
                    .ToListAsync();

                int existingSubscriptionsCount = existingSubscriptions.Count;
                int subscriptionsCount = existingSubscriptionsCount;
                bool limitReached = subscriptionsCount >= Subscription.MaxPerSubscriber;

                if (subscriptionsCount < Subscription.MaxPerSubscriber)
                {
                    foreach (var artist in input.Artists)
                    {
                        if (!existingSubscriptions.Any(s => s.ArtistId == artist.Id))
                        {
                            //Don't use _crudServices.CreateAndSaveAsync, because we only want to call Save once for performance reasons
                            _crudServices.Context.Add(new Subscription
                            {
                                ArtistId = artist.Id,
                                SubscriberId = input.Subscriber.Id
                            });

                            subscriptionsCount++;
                            if (subscriptionsCount >= Subscription.MaxPerSubscriber)
                            {
                                //Reached the maximum limit, don't create any more subscriptions for this Subscriber
                                limitReached = true;
                                break;
                            }
                        }
                    }

                    await _crudServices.Context.SaveChangesAsync();
                }

                var output = new SubscribeToArtistsOutput
                {
                    ErrorMessage = _crudServices.IsValid ? null : _crudServices.GetAllErrors()
                };

                output.SetStatusMessage(existingSubscriptionsCount, subscriptionsCount - existingSubscriptionsCount, limitReached);

                return output;
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

        public async Task<GetSubscriptionsForArtistOutput> GetSubscriptionsForArtist(GetSubscriptionsForArtistInput input)
        {
            if (input.ArtistId <= 0)
                throw new ArgumentException("ArtistId must be a valid Id", "ArtistId");

            try
            {
                var subscriptions = await _crudServices.ReadManyNoTracked<SubscriptionDto>()
                    .Where(s => s.ArtistId == input.ArtistId)
                    .ToListAsync();

                return new GetSubscriptionsForArtistOutput
                {
                    Subscriptions = subscriptions
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArtistId: " + input.ArtistId);
                return new GetSubscriptionsForArtistOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<UnsubscribeFromArtistOutput> UnsubscribeFromArtist(UnsubscribeFromArtistInput input)
        {
            if (input.ArtistId <= 0)
                throw new ArgumentException("ArtistId must be a valid Id", "ArtistId");

            if (String.IsNullOrWhiteSpace(input.UnsubscribeToken))
                throw new ArgumentException("UnsubscribeToken must be set", "UnsubscribeToken");

            try
            {
                var subscription = await _crudServices.ReadSingleAsync<Subscription>(
                    s => s.ArtistId == input.ArtistId && s.Subscriber.UnsubscribeToken == input.UnsubscribeToken);

                if (subscription != null)
                {
                    await _crudServices.DeleteAndSaveAsync<Subscription>(subscription.Id);

                    return new UnsubscribeFromArtistOutput
                    {
                        ErrorMessage = _crudServices.IsValid ? null : _crudServices.GetAllErrors()
                    };
                }
                else
                {
                    return new UnsubscribeFromArtistOutput
                    {
                        ErrorMessage = "Subscription not found. Either you've already unsubscribed, or the URL that got you here wasn't quite right."
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArtistId: {0}, UnsubscribeToken: {1}", input.ArtistId, input.UnsubscribeToken);
                return new UnsubscribeFromArtistOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<UnsubscribeFromAllOutput> UnsubscribeFromAll(UnsubscribeFromAllInput input)
        {
            if (String.IsNullOrWhiteSpace(input.UnsubscribeToken))
                throw new ArgumentException("UnsubscribeToken must be set", "UnsubscribeToken");

            try
            {
                var subscriber = await _crudServices.ReadSingleAsync<Subscriber>(
                    s => s.UnsubscribeToken == input.UnsubscribeToken);

                if (subscriber != null)
                {
                    //This will cascade delete their Subscriptions too
                    await _crudServices.DeleteAndSaveAsync<Subscriber>(subscriber.Id);

                    return new UnsubscribeFromAllOutput
                    {
                        ErrorMessage = _crudServices.IsValid ? null : _crudServices.GetAllErrors()
                    };
                }
                else
                {
                    return new UnsubscribeFromAllOutput
                    {
                        ErrorMessage = "Subscription not found. Either you've already unsubscribed, or the URL that got you here wasn't quite right."
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "UnsubscribeToken: {0}", input.UnsubscribeToken);
                return new UnsubscribeFromAllOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
