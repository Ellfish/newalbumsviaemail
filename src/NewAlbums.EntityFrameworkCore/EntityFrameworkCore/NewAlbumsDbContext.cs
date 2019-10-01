using Microsoft.EntityFrameworkCore;
using NewAlbums.Albums;
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

        public DbSet<Album> Albums { get; set; }
        
        public DbSet<ArtistAlbum> ArtistAlbums { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public NewAlbumsDbContext(DbContextOptions<NewAlbumsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistAlbum>().HasKey(aa => new { aa.ArtistId, aa.AlbumId });
        }
    }
}
