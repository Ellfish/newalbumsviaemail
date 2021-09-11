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
using NewAlbums.Emails.Templates;
using NewAlbums.Emails.Templates.Dto;
using NewAlbums.Spotify;
using NewAlbums.Spotify.Dto;
using NewAlbums.Subscribers;
using NewAlbums.Subscribers.Dto;
using NewAlbums.Subscriptions;
using NewAlbums.Subscriptions.Dto;
using NewAlbums.Utils;
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
        private readonly ISpotifyAppService _spotifyAppService;
        private readonly EmailManager _emailManager;
        private readonly TemplateManager _templateManager;
        private readonly IConfiguration _configuration;

        public SubscriptionController(
            ISubscriberAppService subscriberAppService,
            IArtistAppService artistAppService,
            ISubscriptionAppService subscriptionAppService,
            ISpotifyAppService spotifyAppService,
            EmailManager emailManager,
            TemplateManager templateManager,
            IConfiguration configuration)
        {
            _subscriberAppService = subscriberAppService;
            _artistAppService = artistAppService;
            _subscriptionAppService = subscriptionAppService;
            _spotifyAppService = spotifyAppService;
            _emailManager = emailManager;
            _templateManager = templateManager;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeToArtists([FromBody] SubscribeToArtistsRequest request)
        {
            if (!request.SpotifyArtists.Any())
                return BadRequest(new ApiResponse(400, "Please select at least one artist to subscribe to."));

            //Get the email address from their Spotify account again to see if we can auto-verify their Subscriber email.
            //This is more secure than eg trusting a flag in the request, which could be spoofed.
            //If there are any errors, fall back to assuming we need to verify their email.
            bool emailVerified = false;
            string requestEmail = StringUtils.NormaliseEmailAddress(request.EmailAddress);
            if (!String.IsNullOrWhiteSpace(request.SpotifyAccessToken))
            {
                var emailOutput = await _spotifyAppService.GetUserEmail(new GetUserEmailInput
                {
                    AccessToken = request.SpotifyAccessToken
                });

                if (!emailOutput.HasError && emailOutput.EmailAddress != null && StringUtils.NormaliseEmailAddress(emailOutput.EmailAddress) == requestEmail)
                {
                    emailVerified = true;
                }
            }

            var subscriberOutput = await _subscriberAppService.GetOrCreate(new GetOrCreateSubscriberInput
            {
                EmailAddress = request.EmailAddress,
                EmailAddressVerified = emailVerified
            });

            if (subscriberOutput.HasError)
                return StatusCode(500, new ApiResponse(500, subscriberOutput.ErrorMessage));

            var artistsOutput = await _artistAppService.GetOrCreateMany(new GetOrCreateManyInput
            {
                Artists = request.SpotifyArtists
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

            if (!subscriberOutput.Subscriber.EmailAddressVerified)
            {
                await SendVerificationEmail(subscriberOutput.Subscriber);
            }

            return Ok(new ApiOkResponse(subscriptionOutput));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
        {
            if (request.ArtistId.HasValue && request.ArtistId.Value <= 0)
                return BadRequest(new ApiResponse(400, "Please provide a valid ArtistId."));

            BaseOutput unsubscribeOutput = null;

            if (request.ArtistId.HasValue)
            {
                unsubscribeOutput = await _subscriptionAppService.UnsubscribeFromArtist(new UnsubscribeFromArtistInput
                {
                    ArtistId = request.ArtistId.Value,
                    UnsubscribeToken = request.UnsubscribeToken
                });
            }
            else
            {
                unsubscribeOutput = await _subscriptionAppService.UnsubscribeFromAll(new UnsubscribeFromAllInput
                {
                    UnsubscribeToken = request.UnsubscribeToken
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

        private async Task SendVerificationEmail(SubscriberDto subscriber)
        {
            string verifyUrl = GetVerifyEmailUrl(subscriber);

            var message = new EmailMessage
            {
                BodyText = $"Hello, please click the link below to verify your email address. This is required before you can receive new album emails:\r\n\r\n{verifyUrl}\r\n\r\nThanks for using New Albums via Email.",
                BodyHtml = await GetVerificationEmailHtml(verifyUrl),
                Subject = "Please verify your email address",
                ToAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Address = subscriber.EmailAddress
                    }
                }
            };

            await _emailManager.SendEmail(message);
        }

        private async Task<string> GetVerificationEmailHtml(string verifyUrl)
        {
            var getTemplateInput = new GetTemplateInput
            {
                TemplateType = TemplateTypes.Alert,
                SimpleVariableValues = new Dictionary<string, string>
                {
                    { TemplateVariables.Heading,  $"Verify your email" }
                },
                BodyParagraphs = new List<BodyParagraph>
                {
                    new BodyParagraph { HtmlText = $"Hello, please click the button below to verify your email address. This is required before you can receive new album emails:" },
                    new BodyParagraph { HtmlText = "Verify", ButtonUrl = verifyUrl  },
                    new BodyParagraph { HtmlText = $"Thanks for using New Albums via Email." }
                }
            };

            var template = await _templateManager.GetHtmlEmailTemplate(getTemplateInput);

            return template.ToString();
        }

        private string GetVerifyEmailUrl(SubscriberDto subscriber)
        {
            string baseUrl = _configuration[AppSettingKeys.App.FrontEndRootUrl].TrimEnd('/');
            return $"{baseUrl}/verify-email?emailAddress={subscriber.EmailAddress}&verifyCode={subscriber.EmailVerifyCode}";
        }
    }
}
