using GenericServices;
using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class SubscriptionDto : CreationAuditedEntityDto<long>, ILinkToEntity<Subscription>
    {
        public long SubscriberId { get; set; }

        public long ArtistId { get; set; }
    }
}
