using NewAlbums.Notifications.Dto;
using NewAlbums.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Notifications
{
    public interface INotificationAppService
    {
        Task<NotifySubscribersOutput> NotifySubscribers(NotifySubscribersInput input);
    }
}
