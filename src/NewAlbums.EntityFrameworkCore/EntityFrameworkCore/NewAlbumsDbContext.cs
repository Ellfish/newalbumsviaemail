using Microsoft.EntityFrameworkCore;
using NewAlbums.Artists;
using NewAlbums.Subscribers;
using NewAlbums.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.EntityFrameworkCore
{
    public class NewAlbumsDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public NewAlbumsDbContext(DbContextOptions<NewAlbumsDbContext> options)
            : base(options)
        {

        }
    }
}
