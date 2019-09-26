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
        public virtual long SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual Subscriber Subscriber { get; set; }

        public virtual long ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }
    }
}
