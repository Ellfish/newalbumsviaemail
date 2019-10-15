using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class SubscribeToArtistsOutput : BaseOutput
    {
        public string StatusMessage { get; set; }

        public void SetStatusMessage(int existingSubscriptions, int newSubscriptions, bool limitReached = false)
        {
            string message = $"Subscribed to {newSubscriptions} new artist{(newSubscriptions == 1 ? "" : "s")}. ";

            if (existingSubscriptions > 0)
            {
                message += $"You have existing subscriptions to {existingSubscriptions} artist{(existingSubscriptions == 1 ? "" : "s")}. ";
            }

            if (limitReached)
            {
                message += $"You've now reached the limit of {Subscription.MaxPerSubscriber} artist subscriptions, and can't subscribe to any more (sorry).";
            }

            StatusMessage = message;
        }
    }
}
