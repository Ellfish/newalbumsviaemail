using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class GetOrCreateSubscriberOutput : BaseOutput
    {
        public SubscriberDto Subscriber { get; set; }
    }
}
