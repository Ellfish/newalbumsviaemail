using NewAlbums.Artists;
using NewAlbums.EntityFrameworkCore;
using NewAlbums.Subscribers;
using NewAlbums.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Tests.Helpers
{
    public static class SubscriberTestData
    {
        private static readonly DateTime _createdDate = DateTime.UtcNow;

        public static void SeedDatabaseSubscriberToTwoArtists(this NewAlbumsDbContext context)
        {
            var artists = CreateTwoArtists();
            context.Artists.AddRange(artists);

            var subscriber = CreateSubscriber();
            context.Subscribers.Add(subscriber);

            CreateSubscriptions(subscriber, artists);

            context.SaveChanges();
        }

        private static void CreateSubscriptions(Subscriber subscriber, List<Artist> artists)
        {
            if (subscriber.Subscriptions == null)
                subscriber.Subscriptions = new List<Subscription>();

            foreach (var artist in artists)
            {
                subscriber.Subscriptions.Add(new Subscription
                {
                    Artist = artist,
                    CreatedDate = _createdDate,
                    Subscriber = subscriber
                });
            }
        }

        private static Subscriber CreateSubscriber()
        {
            return new Subscriber
            {
                CreatedDate = _createdDate,
                EmailAddress = "subscriber@newalbumsvia.email"
            };
        }

        private static List<Artist> CreateTwoArtists()
        {
            var artists = new List<Artist>();

            artists.Add(new Artist
            {
                CreatedDate = _createdDate,
                Name = "Crowded House",
                SpotifyId = "7ohlPA8dRBtCf92zaZCaaB"
            });

            artists.Add(new Artist
            {
                CreatedDate = _createdDate,
                Name = "Nils Frahm",
                SpotifyId = "5gqhueRUZEa7VDnQt4HODp"
            });

            return artists;
        }
    }
}
