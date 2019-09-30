using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAlbums.Emails;
using NewAlbums.Notifications.Dto;
using NewAlbums.Subscriptions.Dto;

namespace NewAlbums.Notifications
{
    public class NotificationAppService : BaseAppService, INotificationAppService
    {
        private readonly ICrudServicesAsync _crudServices;
        private readonly EmailManager _emailManager;

        public NotificationAppService(
            ICrudServicesAsync crudServices,
            EmailManager emailManager
            )
        {
            _crudServices = crudServices;
            _emailManager = emailManager;
        }

        public async Task<NotifySubscribersOutput> NotifySubscribers(NotifySubscribersInput input)
        {
            throw new NotImplementedException();
        }
    }
}
