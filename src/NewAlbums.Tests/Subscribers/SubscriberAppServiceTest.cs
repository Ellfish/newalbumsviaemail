using NewAlbums.EntityFrameworkCore;
using NewAlbums.Subscribers;
using NewAlbums.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using TestSupport.EfHelpers;
using Xunit;
using NewAlbums.Artists;
using NewAlbums.Subscriptions;
using Xunit.Extensions.AssertExtensions;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Artists.Dto;
using NewAlbums.Subscriptions.Dto;

namespace NewAlbums.Tests.Subscribers
{
    public class SubscriberAppServiceTest
    {
        private readonly ISubscriberAppService _subscriberAppService;

        [Fact]
        public void EmailSentForNewAlbum()
        {
            //Setup
            var options = SqliteInMemory.CreateOptions<NewAlbumsDbContext>();
            using (var context = new NewAlbumsDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseSubscriberToTwoArtists();

                var utData = context.SetupSingleDtoAndEntities<SubscriberDto>();
                utData.AddSingleDto<ArtistDto>();
                utData.AddSingleDto<SubscriptionDto>();

                var service = new CrudServices(context, utData.ConfigAndMapper);

                //_subscriberAppService = new SubscriberAppService(service, )

                //Attempt
                var dto = service.ReadSingle<Subscription>(1L);

                //Verify
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.Id.ShouldEqual(1);

            }
        }
    }
}
