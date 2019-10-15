using NewAlbums.Artists;
using NewAlbums.Entities;
using NewAlbums.Subscribers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewAlbums.Subscriptions
{
    public class Subscription : CreationAuditedEntity<long>
    {
        /// <summary>
        /// We need some sort of limit to prevent users overloading our database with artists.
        /// 2000 should be a lot more than most users follow, or would choose to subscribe to.
        /// </summary>
        public const int MaxPerSubscriber = 2000;

        public virtual long SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual Subscriber Subscriber { get; set; }

        public virtual long ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }
    }
}
