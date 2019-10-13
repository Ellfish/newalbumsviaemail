using GenericServices.PublicButHidden;
using GenericServices.Setup;
using NewAlbums.Albums.Dto;
using NewAlbums.Artists.Dto;
using NewAlbums.EntityFrameworkCore;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TestSupport.EfHelpers;

namespace NewAlbums.Tests
{
    public abstract class BaseTest : IDisposable
    {
        protected NewAlbumsDbContext DbContext { get; private set; }

        protected CrudServicesAsync CrudServicesAsync { get; private set; }

        public BaseTest()
        {
            var options = SqliteInMemory.CreateOptions<NewAlbumsDbContext>();
            DbContext = new NewAlbumsDbContext(options);
            DbContext.Database.EnsureCreated();

            var utData = DbContext.SetupSingleDtoAndEntities<SubscriberDto>();
            utData.AddSingleDto<ArtistDto>();
            utData.AddSingleDto<AlbumDto>();
            utData.AddSingleDto<SubscriptionDto>();

            CrudServicesAsync = new CrudServicesAsync(DbContext, utData.ConfigAndMapper);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
