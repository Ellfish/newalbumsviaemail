using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewAlbums.Artists;
using NewAlbums.Artists.Dto;
using NewAlbums.Configuration;
using NewAlbums.Emails;
using NewAlbums.Emails.Dto;
using NewAlbums.Subscribers;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Subscriptions;
using NewAlbums.Subscriptions.Dto;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Requests.Subscription;
using NewAlbums.Web.Responses.Common;

namespace NewAlbums.Web.Controllers
{
    [Route("api/[controller]")]
    [RequestValidation]
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriberAppService _subscriberAppService;
        private readonly IArtistAppService _artistAppService;
        private readonly ISubscriptionAppService _subscriptionAppService;
        private readonly EmailManager _emailManager;
        private readonly IConfiguration _configuration;

        public SubscriptionController(
            ISubscriberAppService subscriberAppService,
            IArtistAppService artistAppService,
            ISubscriptionAppService subscriptionAppService,
            EmailManager emailManager,
            IConfiguration configuration)
        {
            _subscriberAppService = subscriberAppService;
            _artistAppService = artistAppService;
            _subscriptionAppService = subscriptionAppService;
            _emailManager = emailManager;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeToArtists([FromBody] SubscribeToArtistsRequest model)
        {
            if (!model.SpotifyArtists.Any())
                return BadRequest(new ApiResponse(400, "Please select at least one artist to subscribe to."));

            var subscriberOutput = await _subscriberAppService.GetOrCreate(new GetOrCreateSubscriberInput
            {
                EmailAddress = model.EmailAddress
            });

            if (subscriberOutput.HasError)
                return StatusCode(500, new ApiResponse(500, subscriberOutput.ErrorMessage));

            var artistsOutput = await _artistAppService.GetOrCreateMany(new GetOrCreateManyInput
            {
                Artists = model.SpotifyArtists
            });

            if (artistsOutput.HasError)
                return StatusCode(500, new ApiResponse(500, artistsOutput.ErrorMessage));

            var subscriptionOutput = await _subscriptionAppService.SubscribeToArtists(new SubscribeToArtistsInput
            {
                Subscriber = subscriberOutput.Subscriber,
                Artists = artistsOutput.Artists
            });

            if (subscriptionOutput.HasError)
                return StatusCode(500, new ApiResponse(500, subscriptionOutput.ErrorMessage));

            if (subscriberOutput.CreatedNewSubscriber)
            {
                await SendNotificationEmail(subscriberOutput.Subscriber.EmailAddress, artistsOutput.Artists.Count);
            }            

            return Ok(new ApiOkResponse(subscriptionOutput.StatusMessage));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest model)
        {
            if (model.ArtistId.HasValue && model.ArtistId.Value <= 0)
                return BadRequest(new ApiResponse(400, "Please provide a valid ArtistId."));

            BaseOutput unsubscribeOutput = null;

            if (model.ArtistId.HasValue)
            {
                unsubscribeOutput = await _subscriptionAppService.UnsubscribeFromArtist(new UnsubscribeFromArtistInput
                {
                    ArtistId = model.ArtistId.Value,
                    UnsubscribeToken = model.UnsubscribeToken
                });
            }
            else
            {
                unsubscribeOutput = await _subscriptionAppService.UnsubscribeFromAll(new UnsubscribeFromAllInput
                {
                    UnsubscribeToken = model.UnsubscribeToken
                });
            }

            if (unsubscribeOutput.HasError)
                return StatusCode(500, new ApiResponse(500, unsubscribeOutput.ErrorMessage));

            return Ok(new ApiOkResponse(true));
        }

        /// <summary>
        /// Notifies the site admin that there was a new subscriber
        /// </summary>
        private async Task SendNotificationEmail(string subscriberEmailAddress, int artistsCount)
        {
            string adminEmailAddress = _configuration[AppSettingKeys.App.AdminEmailAddress];
            if (String.IsNullOrWhiteSpace(adminEmailAddress))
            {
                Logger.LogInformation("Not sending new subscriber notification email, no AdminEmailAddress configured.");
                return;
            }

            string adminFullName = _configuration[AppSettingKeys.App.AdminFullName];

            var message = new EmailMessage
            {
                BodyText = $"New subscriber: {subscriberEmailAddress}, subscribed to: {artistsCount} artists",
                Subject = "New subscriber to New Albums via Email",
                ToAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Address = adminEmailAddress,
                        DisplayName = adminFullName
                    }
                }
            };

            await _emailManager.SendEmail(message);
        }
    }
}