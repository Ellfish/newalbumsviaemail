using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.Extensions.Logging;
using NewAlbums.Subscribers.Dto;

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
            if (String.IsNullOrWhiteSpace(input.EmailAddress))
                throw new ArgumentException("EmailAddress must be a valid email address", "EmailAddress");

            try
            {
                SubscriberDto subscriberDto = null;

                subscriberDto = await _crudServices.ReadSingleAsync<SubscriberDto>(s => s.EmailAddress == input.EmailAddress);
                if (subscriberDto == null)
                {
                    subscriberDto = await _crudServices.CreateAndSaveAsync(new SubscriberDto { EmailAddress = input.EmailAddress });
                }

                return new GetOrCreateSubscriberOutput
                {
                    Subscriber = subscriberDto,
                    ErrorMessage = subscriberDto == null ? "Error creating subscriber in database" : null
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
