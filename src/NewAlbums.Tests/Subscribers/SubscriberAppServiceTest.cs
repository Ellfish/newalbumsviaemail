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
using NewAlbums.Tests.Fixtures;
using NewAlbums.Emails;
using System.Threading.Tasks;
using NewAlbums.Albums.Dto;
using NewAlbums.Spotify.Dto;
using NewAlbums.Emails.Templates;

namespace NewAlbums.Tests.Subscribers
{
    [Collection("Shared collection")]
    public class SubscriberAppServiceTest : BaseTest
    {
        private readonly SharedFixture _sharedFixture;

        private readonly ISubscriberAppService _subscriberAppService;

        public SubscriberAppServiceTest(SharedFixture sharedFixture)
        {
            //Constructor injected by xunit
            _sharedFixture = sharedFixture;

            var emailManager = new EmailManager(_sharedFixture.PathProvider, _sharedFixture.Configuration);
            var templateManager = new TemplateManager(_sharedFixture.Configuration);
            _subscriberAppService = new SubscriberAppService(CrudServicesAsync, emailManager, templateManager, _sharedFixture.Configuration);
        }

        [Fact]
        public async Task EmailSentForNewAlbum()
        {
            //Setup
            DbContext.SeedDatabaseSubscriberToTwoArtists();

            //Attempt and Verify
            var subscriptionDto = await CrudServicesAsync.ReadSingleAsync<SubscriptionDto>(1L);
            
            CrudServicesAsync.IsValid.ShouldBeTrue(CrudServicesAsync.GetAllErrors());
            subscriptionDto.Id.ShouldEqual(1);

            var artistDto = await CrudServicesAsync.ReadSingleAsync<ArtistDto>(1L);

            var output = await _subscriberAppService.NotifySubscribers(new NotifySubscribersInput
            {
                Subscriptions = new List<SubscriptionDto> { subscriptionDto },
                Artist = artistDto,
                Album = new AlbumDto
                {
                    CreatedDate = DateTime.UtcNow,
                    Name = "Testing 123",
                    ReleaseDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"),
                    SpotifyId = "1iBbBHhftbF1lJmOPs4D18",
                    Image = new SpotifyImageDto
                    {
                        Width = 300,
                        Height = 300,
                        Url = "https://i.scdn.co/image/ab67616d00001e023587a048cf3e56f0efc56990"
                    }
                }
            });

            output.HasError.ShouldBeFalse(output.ErrorMessage);

            //TODO: verify that the email exists on disk?
        }
    }
}
