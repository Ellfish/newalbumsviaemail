using NewAlbums.Entities;
using NewAlbums.Subscriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscribers
{
    public class Subscriber : CreationAuditedEntity<long>
    {
        public const int MAX_LENGTH_EMAIL_ADDRESS = 254;
        public const int MAX_LENGTH_UNSUBSCRIBE_TOKEN = 36;

        [MaxLength(MAX_LENGTH_EMAIL_ADDRESS)]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Simple way to allow users to unsubscribe from emails without requiring authentication
        /// </summary>
        [MaxLength(MAX_LENGTH_UNSUBSCRIBE_TOKEN)]
        public virtual string UnsubscribeToken { get; private set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public Subscriber()
        {
            UnsubscribeToken = Guid.NewGuid().ToString();
        }
    }
}
