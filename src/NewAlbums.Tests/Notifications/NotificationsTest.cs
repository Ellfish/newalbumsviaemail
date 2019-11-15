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

namespace NewAlbums.Tests.Notifications
{
    [Collection("Shared collection")]
    public class NotificationsTest : BaseTest
    {
        private readonly SharedFixture _sharedFixture;

        public NotificationsTest(SharedFixture sharedFixture)
        {
            //Constructor injected by xunit
            _sharedFixture = sharedFixture;

        }        
    }
}
