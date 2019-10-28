using NewAlbums.Subscribers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Subscribers
{
    public interface ISubscriberAppService
    {
        Task<GetOrCreateSubscriberOutput> GetOrCreate(GetOrCreateSubscriberInput input);

        Task<UpdateSubscriberOutput> Update(UpdateSubscriberInput input);

        Task<CheckEmailVerificationOutput> CheckEmailVerification(CheckEmailVerificationInput input);

        Task<NotifySubscribersOutput> NotifySubscribers(NotifySubscribersInput input);
    }
}
