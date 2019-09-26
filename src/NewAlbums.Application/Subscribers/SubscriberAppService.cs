using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.Extensions.Logging;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Utils;

namespace NewAlbums.Subscribers
{
    public class SubscriberAppService : BaseAppService, ISubscriberAppService
    {
        private readonly ICrudServicesAsync _crudServices;

        public SubscriberAppService(ICrudServicesAsync crudServices)
        {
            _crudServices = crudServices;
        }

        public async Task<GetOrCreateSubscriberOutput> GetOrCreate(GetOrCreateSubscriberInput input)
        {
            try
            {
                SubscriberDto subscriberDto = null;
                bool newSubscriber = false;
                string normalisedEmail = StringUtils.NormaliseEmailAddress(input.EmailAddress);

                subscriberDto = await _crudServices.ReadSingleAsync<SubscriberDto>(s => s.EmailAddress == normalisedEmail);
                if (subscriberDto == null)
                {
                    subscriberDto = await _crudServices.CreateAndSaveAsync(new SubscriberDto { EmailAddress = normalisedEmail });
                    if (!_crudServices.IsValid)
                    {
                        return new GetOrCreateSubscriberOutput
                        {
                            ErrorMessage = _crudServices.GetAllErrors()
                        };
                    }

                    newSubscriber = true;
                }

                return new GetOrCreateSubscriberOutput
                {
                    Subscriber = subscriberDto,
                    CreatedNewSubscriber = newSubscriber,
                    ErrorMessage = subscriberDto == null ? $"Error creating subscriber with email '{normalisedEmail}' in database" : null
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EmailAddress: " + input.EmailAddress);
                return new GetOrCreateSubscriberOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
