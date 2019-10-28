﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewAlbums.EntityFrameworkCore;

namespace NewAlbums.Migrations
{
    [DbContext(typeof(NewAlbumsDbContext))]
    partial class NewAlbumsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NewAlbums.Albums.Album", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("ReleaseDate")
                        .HasMaxLength(10);

                    b.Property<string>("SpotifyId")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("NewAlbums.Albums.ArtistAlbum", b =>
                {
                    b.Property<long>("ArtistId");

                    b.Property<long>("AlbumId");

                    b.HasKey("ArtistId", "AlbumId");

                    b.HasIndex("AlbumId");

                    b.ToTable("ArtistAlbums");
                });

            modelBuilder.Entity("NewAlbums.Artists.Artist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("SpotifyId")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("NewAlbums.Subscribers.Subscriber", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(254);

                    b.Property<bool>("EmailAddressVerified");

                    b.Property<string>("EmailVerifyCode")
                        .HasMaxLength(16);

                    b.Property<string>("UnsubscribeToken")
                        .HasMaxLength(36);

                    b.HasKey("Id");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("NewAlbums.Subscriptions.Subscription", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ArtistId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<long>("SubscriberId");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("NewAlbums.Albums.ArtistAlbum", b =>
                {
                    b.HasOne("NewAlbums.Albums.Album", "Album")
                        .WithMany("Artists")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NewAlbums.Artists.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NewAlbums.Subscriptions.Subscription", b =>
                {
                    b.HasOne("NewAlbums.Artists.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NewAlbums.Subscribers.Subscriber", "Subscriber")
                        .WithMany("Subscriptions")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
