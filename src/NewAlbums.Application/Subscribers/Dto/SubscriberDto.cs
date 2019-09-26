using GenericServices;
using NewAlbums.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class SubscriberDto : CreationAuditedEntityDto<long>, ILinkToEntity<Subscriber>
    {
        public string EmailAddress { get; set; }

        [ReadOnly(true)]
        public string UnsubscribeToken { get; set; }
    }
}
