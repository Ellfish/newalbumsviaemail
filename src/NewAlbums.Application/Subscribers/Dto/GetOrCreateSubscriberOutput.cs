using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class GetOrCreateSubscriberOutput : BaseOutput
    {
        public SubscriberDto Subscriber { get; set; }

        /// <summary>
        /// True if we just created a new subscriber, not retrieved an existing
        /// </summary>
        public bool CreatedNewSubscriber { get; set; }
    }
}
